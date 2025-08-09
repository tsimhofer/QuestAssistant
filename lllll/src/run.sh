    #!/bin/bash
    echo "Starting QuestAssistant Add-on..."

    # Navigiere in den Ordner deiner Anwendung im Docker-Container
    cd /app

    # Führe die .NET-Anwendung aus
    dotnet QuestAssistant.dll # DLL-Name hier anpassen
    