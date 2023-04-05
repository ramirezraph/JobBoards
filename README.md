# JobBoards - PJLI Connex
PJLI Connex is a web application developed with .NET technologies that offers a variety of features which enhances the job posting and job search experience when looking for qualified candidates for PJLI job openings. It is designed to simplify the job search process by connecting job seekers with relevant job opportunities in PJLI. 

### Technologies:
- C#, .NET 7
- .NET Core MVC Web Application
- .NET Core Web API
- SQL Server 2022
- Docker and Kubernetes

# Members
- Joagne Ann F. Fernando
- Prince Joshua M. Guintu
- Raphael Isiah L. Ramirez

# Setup
Firstly, make sure you have docker and kubernetes installed and running on your computer. Once done, follow the step below.
1. Clone the repository.
    ```bash
    git clone https://github.com/ramirezraph/JobBoards.git
    ```

2. Go to the project's root directory.
    ```bash
    cd JobBoards
    ```

3. Go to the `k8` directory and apply the kubernetes file.
    ```
    cd k8s
    kubectl apply -f .\deployment.yaml
    ```

4. Check if all the services is running with this command:
    ```bash
    kubectl get all -n jobboard
    ```

5. To access the web applications, you can use the `kubectl port-forward` command to forward the webapp or webapi service port to your local machine.

    To port-forward the web application:
    ```bash
    kubectl port-forward svc/jobboardswebapp <port>:5001 -n jobboard
    ```

    To port-forward the web api:
    ```bash
    kubectl port-forward svc/jobboardsapi <port>:6001 -n jobboard
    ```

    Then, you can now access the web application or web api in your browser by going to `http://localhost:<port>`


# Screenshots
