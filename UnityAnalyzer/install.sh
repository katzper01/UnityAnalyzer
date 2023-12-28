dotnet publish -o bin/app/ --self-contained
cp bin/app/UnityAnalyzer /usr/bin/UnityAnalyzer
rm -rf bin/app