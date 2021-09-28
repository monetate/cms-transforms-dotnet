#!/usr/bin/env groovy

REPO_NAME = 'cms-transforms-dotnet'
TEMPDIR = env.TMPDIR ?: '/var/lib/jenkins/tmp'
VENV = 'virtualenv'

//@Library('monetate-jenkins-pipeline')
//import org.monetate.Slack
//def slack = new Slack(steps, REPO_NAME)

//slack.success(this, ':pipeline: Pipeline started')

node('node-v8') {
    try {
        def venvDir = "${TEMPDIR}/${env.BUILD_TAG}-check-account"

        stage('Checkout source') {
            checkout scm
        }


        nvm('12') {
            withEnv(["PATH=${venvDir}/bin:${PATH}"]) {
		        stage('Test') {
		            sh 'cd cms-transforms-c-sharp/CmsTransformLibrary && dotnet test'
		        }
            }
	    }

        if (manager.logContains('.*WARNING.*')) {
            currentBuild.result = result.UNSTABLE
        } else {
            currentBuild.result = result.SUCCESS
        }

    } finally {
        if (currentBuild.result == null) {
            currentBuild.result = result.FAILURE
            echo "Build result was null.  Defaulted to FAILURE."
        }

//        slack.currentResult(this, ':pipeline: Pipeline finished')
    }
}