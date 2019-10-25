 -> Create a database from csv
    - Download and install the sqlite3
    - On the prompt, go to projet root. Go to the DatabaseFolder folder and type:
        - sqlite3 DatabaseFolder
        - .mode csv
        - .import stores.csv tblStores

    Theses steps will create a sqlite database based on the csv file.

-> To run locally
    - install the .net core SDK and .Net core runtime
    - Go to the root folder (ShippAPI) and type on prompt:
        - dotnet restore
        - dotnet build
        - dotnet run
    - Go to the browser and type:
        - https://localhost:5001/V1/stores?long=-73.418&lat=44.750828

-> Also, you can run locally by the docker image
    - open the prompt and type:
        - docker pull tallesvaliatti/shipp_api:1.0
        - docker run -d -p 1789:80 --name shippapi tallesvaliatti/shipp_api:1.0
    - Go to the browser and type:
        - 0.0.0.0:1789/V1/stores?long=-73.418&lat=44.750828

-> Otherwise, access the shipp API deployed on my swarm cluster
     - Go to the browser and type:
        - 167.71.121.156:1789/V1/stores?long=-73.418&lat=44.750828

-> To run the tests locally
    - install the .net core SDK and .Net core runtime
    - Go to the root folder (ShippAPItests) and type on prompt:
        - dotnet restore
        - dotnet build
        - dotnet test    

obs:
    - Just basic unit tests were implemented
    - On the github files, log.txt already have logs.
    - On the github files, database.db is already created.
    - I have implemented the two middleware requested by the test.
    - If you have some questions, please contact me: 27 99898-3908.

