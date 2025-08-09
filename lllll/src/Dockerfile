    # Stage 1: Build the application
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    COPY ["QuestAssistant.csproj", "."] # Projektname hier anpassen
    RUN dotnet restore "QuestAssistant.csproj"
    COPY . .
    WORKDIR "/src"
    RUN dotnet publish "QuestAssistant.csproj" -c Release -o /app/publish /p:UseAppHost=false

    # Stage 2: Create the final runtime image
    FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
    WORKDIR /app
    COPY --from=build /app/publish .

    # Kopiere die credentials.json (muss im Projektordner liegen)
    COPY credentials.json .
    # token.json wird nach dem ersten Start generiert und bleibt im Volume persistiert

    # Wenn du eine REST-API verwendest, expose den Port
    # EXPOSE 5000

    # Startbefehl für deine Anwendung
    ENTRYPOINT ["dotnet", "QuestAssistant.dll"] # DLL-Name hier anpassen
    