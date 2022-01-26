def message = "";
pipeline {
    agent any	
    environment {
        BuildConfiguration='Debug'
        ImageName="registry.gitlab.com/rayprosoft/turnkey-gaming-system/validatecode-api"
        GitUrl = "gitlab.com/rayprosoft/tgs-validatecode.git"
        ImageTagUrl = "https://gitlab.com/api/v4/projects/20478010/registry/repositories/2154529/tags"
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
                            env.request_cpu = "5m"
                            env.request_memory = "100Mi"
                            env.limit_cpu = "50m"
                            env.limit_memory = "500Mi"
                            env.autoscaling_enabled = "true"
                            env.targetCPUUtilizationPercentage = "800"
                            env.targetMemoryUtilizationPercentage = "400"
                            break;
                        case ['release', 'origin/release']:
                            env.kube_ns = "staging";
                            env.k8s_config = 'stag-k8s-config';
                            env.request_cpu = "5m"
                            env.request_memory = "100Mi"
                            env.limit_cpu = "50m"
                            env.limit_memory = "500Mi"
                            env.autoscaling_enabled = "false"
                            env.targetCPUUtilizationPercentage = "800"
                            env.targetMemoryUtilizationPercentage = "400"
                            break;
                        default:
                            env.kube_ns = "dev";
                            env.k8s_config = 'k8s-kube-config'
                            env.request_cpu = "5m"
                            env.request_memory = "100Mi"
                            env.limit_cpu = "50m"
                            env.limit_memory = "500Mi"
                            env.autoscaling_enabled = "false"
                            env.targetCPUUtilizationPercentage = "800"
                            env.targetMemoryUtilizationPercentage = "400"
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
                        env.commit_url = "${env.GIT_URL}".replace(".git","/-/commit/") + "${env.GIT_COMMIT}"
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
                sh 'dotnet nuget push "ValidateCodeClient/bin/$BuildConfiguration/*.nupkg" --source "raypro" --skip-duplicate'
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
                withKubeConfig([credentialsId: env.k8s_config]) {
                    withCredentials([string(credentialsId: "tgs-${kube_ns}-connectionstring", variable: "tgs_tgs")]) {
                        sh '''
                            helm template deployment/k8s/validatecode-api \
                                --set image.tag=${TAG} \
                                --set connectionStrings.tgs="$tgs_tgs" \
                                --set connectionStrings.redis="$tgs_redis" \
                                --set resources.requests.cpu=${env.request_cpu} \
                        '''
                        sh '''

                            helm template deployment/k8s/validatecode-api \
                                --set image.tag=${TAG} \
                                --set connectionStrings.tgs="$tgs_tgs" \
                                --set connectionStrings.redis="$tgs_redis" \
                            | kubectl -n ${kube_ns} apply -f -
                        '''
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
        //stage('Remove Image Tags') {					
        //    steps {	
		//	    withCredentials([string(credentialsId: "GitLabToken", variable: "GitLabToken")]) {					
        //            sh "curl --request DELETE --data \'name_regex_delete=.*\' --data \'keep_n=10\' --header \'PRIVATE-TOKEN: ${GitLabToken}\' ${ImageTagUrl}"
        //        }
        //    }
        //}
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

def sendTelegram(message) {
    def encodedMessage = URLEncoder.encode(message, "UTF-8")

    withCredentials([string(credentialsId: "telegramToken", variable: "TOKEN"), string(credentialsId: "telegramChatId", variable: "CHAT_ID")]) {

        response = httpRequest (consoleLogResponseBody: true,
                contentType: 'APPLICATION_JSON',
                httpMode: 'GET',
                url: "https://api.telegram.org/bot${TOKEN}/sendMessage?text=${encodedMessage}&chat_id=${CHAT_ID}",
                validResponseCodes: '200')
        return response
    }
}
