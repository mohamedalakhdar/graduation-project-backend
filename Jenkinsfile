pipeline {
    agent {
        label 'jenkins-agent'
    }

    tools {
        jdk 'java22'
    }


    stages {
        stage('clean workspace') {
            steps {
                cleanWs()
            }
        }

        stage('checkout from git') {
            steps {
                git branch: 'master', url: 'https://github.com/mohamedalakhdar/graduation-project-backend.git' ,  credentialsId: 'github-credential'
            }
        }



        stage('trivy fs scan') {
            steps {
                sh 'trivy fs . > trivyfs.txt'
            }
        }

        stage('docker build&push') {
            steps {
                withDockerRegistry(url: 'https://index.docker.io/v1/', credentialsId: 'docker-credential') {
                    sh 'docker build -t backend-image .'
                    sh 'docker tag backend-image mohamedahmedalakhdar/backend-image'
                    sh 'docker push mohamedahmedalakhdar/backend-image'
                }
            }
        }    
        stage('trivy'){
            steps {
                sh 'trivy image mohamedahmedalakhdar/backend-image > trivyimage.txt'
            }
        }    
        
        stage('sonarqube analysis') {
            steps {
               withSonarQubeEnv('sonarqube-dotnet') {
               sh '''
                   export PATH=$PATH:/home/ubuntu/.dotnet/tools

                   dotnet sonarscanner begin /k:"sonar-backend"

                   dotnet build

                   dotnet sonarscanner end
                  '''
               }
           }
        }

        
        stage('quality gate') {
            steps {
                script {
                    waitForQualityGate abortPipeline: false, credentialsId: 'sonarqube-dotnet-credential'
                }
            }
        }

    }
}