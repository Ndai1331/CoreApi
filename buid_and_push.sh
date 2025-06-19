# Build and push API image
echo "Building CoreAPI image for AMD64..."
docker build --platform linux/amd64 -t longnguyen1331/drcore-api:latest -f ./Dockerfile .
echo "Pushing CoreAPI image..."
docker push longnguyen1331/drcore-api:latest
