VERSION=0.0.2

docker run --rm -v "${PWD}:/local" --network host -u $(id -u ${USER}):$(id -g ${USER})  openapitools/openapi-generator-cli generate \
-i http://localhost:5033/swagger/v1/swagger.json \
-g csharp \
-o /local/out --additional-properties=packageName=Coflnet.Sky.Bazaar.Flipper.Client,packageVersion=$VERSION,licenseId=MIT

cd out
path=src/Coflnet.Sky.Bazaar.Flipper.Client/Coflnet.Sky.Bazaar.Flipper.Client.csproj
sed -i 's/GIT_USER_ID/Coflnet/g' $path
sed -i 's/GIT_REPO_ID/SkyBazaarFlipper/g' $path
sed -i 's/>OpenAPI/>Coflnet/g' $path
sed -i 's@annotations</Nullable>@annotations</Nullable>\n    <PackageReadmeFile>README.md</PackageReadmeFile>@g' $path
sed -i 's@Remove="System.Web" />@Remove="System.Web" />\n    <None Include="../../../../README.md" Pack="true" PackagePath="\"/>@g' $path

dotnet pack
cp src/Coflnet.Sky.Bazaar.Flipper.Client/bin/Release/Coflnet.Sky.Bazaar.Flipper.Client.*.nupkg ..
