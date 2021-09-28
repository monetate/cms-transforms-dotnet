#!/usr/bin/env groovy
REPO_NAME = 'cms-transforms-dotnet'
TEMPDIR = env.TMPDIR ?: '/var/lib/jenkins/tmp'
VENV = 'virtualenv'
//@Library('monetate-jenkins-pipeline')
//import org.monetate.Slack
//def slack = new Slack(steps, REPO_NAME)

//slack.success(this, ':pipeline: Pipeline started')

node {
    withDotNet {
        stages {
            stage('Checkout Source') {
                checkout scm
            }

            stage('Run Tests') {
                when {
                    anyOf {
                        branch 'main';
                        branch 'develop';
                    }
                }
                steps {
                    githubNotify context:'Running Tests', status: 'PENDING', description:'Running CMS Transform Tests'
                    dir('cms-transforms-c-sharp/CmsTransformTests') {
                        sh 'dotnet test'
                    }

                    post {
                        success {
                            githubNotify context:'Running Tests', status: 'SUCCESS', description:'All tests succeeded'
                        }
                        failure {
                            githubNotify context:'Running Tests', status: 'FAILURE', description:'Tests failed'
                        }
                    }
                }
            }

            stage('Build') {
                when {
                    anyOf {
                        branch 'main';
                        branch 'develop';
                    }
                }

                steps {
                    githubNotify context:'Attempting Build', status: 'PENDING', description:'Trying to build nuget package'
                    dir('cms-transforms-c-sharp/CmsTransformLibrary') {
                        sh 'dotnet pack --configuration=Release'
                    }

                    post {
                        success {
                            githubNotify context:'Attempting Build', status: 'SUCCESS', description:'Project successfully built'
                        }
                        failure {
                            githubNotify context:'Attempting Build', status: 'FAILURE', description:'Project failed being built'
                        }
                    }
                }
            }

            // Steps to upload pipeline to artifactory/nuget go here!

            if (manager.logContains('.*WARNING.*')) {
                currentBuild.result = result.UNSTABLE
            } else {
                currentBuild.result = result.SUCCESS
            }
        }
    }
    post {
        if (currentBuild.result == null) {
            currentBuilt.result = result.FAILURE
        }

        always {
            echo "Pipeline has finished."
        }
        failure {
            echo "Result was failure."
        }
    }
}