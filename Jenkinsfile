@Library('global-shared-library') _
def message = "";
pipeline {
    agent any	
    environment {
        BuildConfiguration='Debug'
        ImageName="harbor.rpgp.cc/tgs/validatecode-api"
    }
    stages {
        stage('Prepare') {
            steps {
                sh "echo 'Branch: $GIT_BRANCH'";
                script {
                    switch(env.GIT_BRANCH)
                    {
                        case ['main', 'origin/main']:
                            env.kube_ns = "prod";
                            env.k8s_config = 'stag-k8s-config';
                            break;
                        case ['release', 'origin/release']:
                            env.kube_ns = "staging";
                            env.k8s_config = 'stag-k8s-config';
                            break;
                        default:
                            env.kube_ns = "dev";
                            env.k8s_config = 'k8s-kube-config'
                            break;
                    }
                }

                configFileProvider([
                    configFile(
                        fileId: 'tgs.' + env.kube_ns , 
                        variable: 'tgsconfig')]) {
                    script {
                        def envItems = readProperties file: "$tgsconfig";
                        envItems.each { key, value -> 
                            env."tgs_${key}" = value;
                        }
                        env.TAG = env.kube_ns + '-' + sh(script: "echo `date +%Y%m%d%H%M`", returnStdout: true).trim()
                        message += "\nBranch: ${env.GIT_BRANCH}" 
                        message += "\nTag: ${env.TAG}"
                        message += "\nEnv: ${env.kube_ns}"
                        env.commit_url = "${env.GIT_URL}".replace("git@gitlab.com:","https://gitlab.com/")
                            .replace(".git","/-/commit/") + "${env.GIT_COMMIT}"
                        message += "\n Commit: ${env.commit_url}"
                    }
				}
                sh "echo tag: $TAG"
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build -c $BuildConfiguration'
            }
        }
        stage('Test') {
            steps {
                sh 'dotnet test -c $BuildConfiguration --logger trx --no-build'
            }
        }
        stage('Push packages') {
	        steps {
                sh 'dotnet pack -c $BuildConfiguration "ValidateCodeClient/ValidateCodeClient.csproj" --no-build'
                sh 'dotnet nuget push "ValidateCodeClient/bin/$BuildConfiguration/*.nupkg" -s "raypro-baget" -k admin --skip-duplicate'
            }
        }
        stage('Build Docker Image') {
            steps {
                sh 'dotnet build "ValidateCodeApi/ValidateCodeApi.csproj" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
            }
        }
        stage('Push Docker Image') {
            steps {
                sh 'docker push $ImageName:${TAG}'
            }
        }
        stage('deploy') {
            when {
                expression {
                    return env.kube_ns == 'prod';
                }
            }
            steps {
                script {
                     input message: 'ready to go?', ok: 'yes';
                }       
                sh "echo start deploy"
            }
        }
        stage('Deploy validatecode-api') {
            steps {
                script {
                    withKubeConfig([credentialsId: env.k8s_config]) {
                        
                        def resources = getResources(env.k8s_config, env.kube_ns, "validatecode-api")
                        def dedicated = getTgsSettings(env.k8s_config, env.kube_ns, "dedicated")
                        def node_selector = getTgsSettings(env.k8s_config, env.kube_ns, "node-selector")

                        println resources
                        println dedicated
                        println node_selector

                        sh """
                            cat << EOF | yq e -P | helm template deployment/k8s/validatecode-api \
                                -n ${kube_ns} \
                                --set image.tag=${TAG} \
                                -f -
                                {
                                    "nodeSelector": $node_selector,
                                    "tolerations": [$dedicated],
                                    "resources": $resources
                                }
                        """
                        sh """
                            cat << EOF | yq e -P | helm template deployment/k8s/validatecode-api \
                                -n ${kube_ns} \
                                --set image.tag=${TAG} \
                                -f - \
                            | kubectl -n ${kube_ns} apply -f -
                            {
                                "nodeSelector": $node_selector,
                                "tolerations": [$dedicated],
                                "resources": $resources
                            }
                        """
                    }
                }
            }
        }	
        stage('Add Tag on Prod') {
            when {
                expression {
                    return env.kube_ns == 'prod';
                }
            }		
            steps {	
                withCredentials([[$class: 'UsernamePasswordMultiBinding', credentialsId:'gitssh-user-pwd', usernameVariable: 'USERNAME', passwordVariable: 'PASSWORD']]) {
				    sh "git tag -a ${TAG} -m ${TAG}"
                    sh "git push https://$USERNAME:$PASSWORD@${GitUrl} --tags"
                }
            }
        }	
    }
    post {
        always {
            step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
        }
        success {
             sendTelegram("Pipeline: ${currentBuild.fullDisplayName} OK! ${message} \n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }
        failure {
            sendTelegram("<b>Failed Pipeline:</b> ${currentBuild.fullDisplayName} Something is wrong with ${env.BUILD_URL}/console\n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }
    }
}
