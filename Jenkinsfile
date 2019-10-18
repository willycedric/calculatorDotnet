#!groovy
pipeline {
// agent {
//   docker {
//     image 'jenkinsslave:latest'
//     registryUrl 'http://8598567586.dkr.ecr.us-west-2.amazonaws.com'
//     registryCredentialsId 'ecr:us-east-1:3435443545-5546566-567765-3225'
//     args '-v /home/centos/.ivy2:/home/jenkins/.ivy2:rw -v jenkins_opt:/usr/local/bin/opt -v jenkins_apijenkins:/home/jenkins/config -v jenkins_logs:/var/logs -v jenkins_awsconfig:/home/jenkins/.aws --privileged=true -u jenkins:jenkins'
//   }
// }
agent any
environment {
    APP_NAME = 'calculatorDotnet'
    BUILD_NUMBER = "${env.BUILD_NUMBER}"
    IMAGE_VERSION="v_${BUILD_NUMBER}"
    GIT_URL="https://github.com/willycedric/${APP_NAME}.git"
    //GIT_CRED_ID='izleka2IGSTDK+MiYOG3b3lZU9nYxhiJOrxhlaJ1gAA='
    REPOURL = 'cL5nSDa+49M.dkr.ecr.us-east-1.amazonaws.com'
    SBT_OPTS='-Xmx1024m -Xms512m'
    JAVA_OPTS='-Xmx1024m -Xms512m'
    WS_PRODUCT_TOKEN='FJbep9fKLeJa/Cwh7IJbL0lPfdYg7q4zxvALAxWPLnc='
    WS_PROJECT_TOKEN='zwzxtyeBntxX4ixHD1iE2dOr4DVFHPp7D0Czn84DEF4='
    HIPCHAT_TOKEN = 'SpVaURsSTcWaHKulZ6L4L+sjKxhGXCkjSbcqzL42ziU='
    HIPCHAT_ROOM = 'NotificationRoomName'
}

// options {
//     buildDiscarder(logRotator(artifactDaysToKeepStr: '', artifactNumToKeepStr: '', daysToKeepStr: '10', numToKeepStr: '20'))
//     timestamps()
//     retry(3)
//     timeout time:10, unit:'MINUTES'
// }
parameters {
    string(defaultValue: "develop", description: 'Branch Specifier', name: 'SPECIFIER')
    booleanParam(defaultValue: false, description: 'Deploy to QA Environment ?', name: 'DEPLOY_QA')
    booleanParam(defaultValue: false, description: 'Deploy to UAT Environment ?', name: 'DEPLOY_UAT')
    booleanParam(defaultValue: false, description: 'Deploy to PROD Environment ?', name: 'DEPLOY_PROD')
}
stages {
    stage("Initialize") {
        steps {
            script {
                notifyBuild('STARTED')
                echo "${BUILD_NUMBER} - ${env.BUILD_ID} on ${env.JENKINS_URL}"
                echo "Branch Specifier :: ${params.SPECIFIER}"
                echo "Deploy to QA? :: ${params.DEPLOY_QA}"
                echo "Deploy to UAT? :: ${params.DEPLOY_UAT}"
                echo "Deploy to PROD? :: ${params.DEPLOY_PROD}"
                //sh 'rm -rf target/universal/*.zip'
            }
        }
    }
stage('Checkout') {
    steps {
        git branch: "${params.SPECIFIER}", url: "${GIT_URL}"
    }
}
stage('Build') {
            steps {
                echo 'Build solution'
                // sh '/usr/local/bin/opt/bin/sbtGitActivator; /usr/local/bin/opt/play-2.5.10/bin/activator -Dsbt.global.base=.sbt -Dsbt.ivy.home=/home/jenkins/.ivy2 -Divy.home=/home/jenkins/.ivy2 compile coverage test coverageReport coverageOff dist'
                sh 'dotnet build'
            }
        }
stage('Unit Test') {
            steps {
                echo 'Run Unit test'
                // sh '/usr/local/bin/opt/bin/sbtGitActivator; /usr/local/bin/opt/play-2.5.10/bin/activator -Dsbt.global.base=.sbt -Dsbt.ivy.home=/home/jenkins/.ivy2 -Divy.home=/home/jenkins/.ivy2 compile coverage test coverageReport coverageOff dist'
                sh "dotnet test --logger 'trx;LogFileName=somename.trx'"
            }
        }



}

    post {
        /*
         * These steps will run at the end of the pipeline based on the condition.
         * Post conditions run in order regardless of their place in the pipeline
         * 1. always - always run
         * 2. changed - run if something changed from the last run
         * 3. aborted, success, unstable or failure - depending on the status
         */
        always {
            echo "I AM ALWAYS first"
            //notifyBuild("${currentBuild.currentResult}")
        }
        aborted {
            echo "BUILD ABORTED"
        }
        success {
            echo "BUILD SUCCESS"
            echo "Keep Current Build If branch is master"
//            keepThisBuild()
        }
        unstable {
            echo "BUILD UNSTABLE"
        }
        failure {
            echo "BUILD FAILURE"
            sh "dotnet test --logger 'trx;LogFileName=somename.trx'"
        }
    }
}
def keepThisBuild() {
    currentBuild.setKeepLog(true)
    currentBuild.setDescription("Test Description")
}

def getShortCommitHash() {
    return sh(returnStdout: true, script: "git log -n 1 --pretty=format:'%h'").trim()
}

def getChangeAuthorName() {
    return sh(returnStdout: true, script: "git show -s --pretty=%an").trim()
}

def getChangeAuthorEmail() {
    return sh(returnStdout: true, script: "git show -s --pretty=%ae").trim()
}

def getChangeSet() {
    return sh(returnStdout: true, script: 'git diff-tree --no-commit-id --name-status -r HEAD').trim()
}

def getChangeLog() {
    return sh(returnStdout: true, script: "git log --date=short --pretty=format:'%ad %aN <%ae> %n%n%x09* %s%d%n%b'").trim()
}

def getCurrentBranch () {
    return sh (
            script: 'git rev-parse --abbrev-ref HEAD',
            returnStdout: true
    ).trim()
}

def isPRMergeBuild() {
    return (env.BRANCH_NAME ==~ /^PR-\d+$/)
}

def notifyBuild(String buildStatus = 'STARTED') {
    // build status of null means successful
    buildStatus = buildStatus ?: 'SUCCESS'

    def branchName = getCurrentBranch()
    def shortCommitHash = getShortCommitHash()
    def changeAuthorName = getChangeAuthorName()
    def changeAuthorEmail = getChangeAuthorEmail()
    def changeSet = getChangeSet()
    def changeLog = getChangeLog()

    // Default values
    def colorName = 'RED'
    def colorCode = '#FF0000'
    def subject = "${buildStatus}: '${env.JOB_NAME} [${env.BUILD_NUMBER}]'" + branchName + ", " + shortCommitHash
    def summary = "Started: Name:: ${env.JOB_NAME} \n " +
            "Build Number: ${env.BUILD_NUMBER} \n " +
            "Build URL: ${env.BUILD_URL} \n " +
            "Short Commit Hash: " + shortCommitHash + " \n " +
            "Branch Name: " + branchName + " \n " +
            "Change Author: " + changeAuthorName + " \n " +
            "Change Author Email: " + changeAuthorEmail + " \n " +
            "Change Set: " + changeSet

    if (buildStatus == 'STARTED') {
        color = 'YELLOW'
        colorCode = '#FFFF00'
    } else if (buildStatus == 'SUCCESS') {
        color = 'GREEN'
        colorCode = '#00FF00'
    } else {
        color = 'RED'
        colorCode = '#FF0000'
    }

    // Send notifications
    // hipchatSend(color: color, notify: true, message: summary, token: "${env.HIPCHAT_TOKEN}",
    //     failOnError: true, room: "${env.HIPCHAT_ROOM}", sendAs: 'Jenkins', textFormat: true)
if (buildStatus == 'FAILURE') {
       // emailext attachLog: true, body: summary, compressLog: true, recipientProviders: [brokenTestsSuspects(), brokenBuildSuspects(), culprits()], replyTo: 'noreply@yourdomain.com', subject: subject, to: 'mpatel@yourdomain.com'
    }
}