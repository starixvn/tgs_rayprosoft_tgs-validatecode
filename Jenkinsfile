pipeline {
    agent any
	
	environment {
        BuildConfiguration='Debug'
		ImageName="registry.gitlab.com/rayprosoft/turnkey-gaming-system/validatecode-api"
	}

    stages {
        stage('Prepare') {
            steps {
                sh "echo 'Branch: $GIT_BRANCH'";
                script {
                    switch(env.GIT_BRANCH)
                    {
                        case 'origin/main':
                            env.kube_ns = "prod";
							env.k8s_config = 'stag-k8s-config';
                            break;
                        case 'origin/release':
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

		//stage('deploy') {
        //    when {
        //        expression {
        //            return env.kube_ns != 'dev';
        //        }
        //    }

        //    steps {
        //        script {
        //             input message: 'ready to go?', ok: 'yes';
        //        }
                
        //        sh "echo start deploy"
        //    }
        //}

		stage('Deploy validatecode-api') {
            steps {
                withKubeConfig([credentialsId: env.k8s_config]) {
                    sh '''
                        helm template deployment/k8s/validatecode-api \
                            --set image.tag=${TAG} \
                            --set connectionStrings.tgs="$tgs_tgs" \
                            --set connectionStrings.redis="$tgs_redis" \

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

	post {
		always {
			step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
		}
	}
}
