using Amazon.S3;
using Amazon.S3.Model;

namespace AWS
{
    public static class S3 {
        public static async Task<String> UploadS3Image() {

            IAmazonS3 client = new AmazonS3Client(region: Amazon.RegionEndpoint.USEast1, credentials: CredentialsLoader.GetMachineCredentials() );

            string bucketName = "kare-learning-qa";
            string objectName = "charmander.png";
            string filepath = "/Users/jaredhernandez/Desktop/KARE/s3/AWS/charmander.png";
            var request = new PutObjectRequest 
            {
                BucketName = bucketName,
                Key = objectName,
                FilePath = filepath
            };

            var response = await client.PutObjectAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Successfully uploaded {objectName} to {bucketName}.");
                return String.Format("https://{0}.s3.us-east-1.amazonaws.com/{1}", bucketName, objectName);
            }
            else
            {
                Console.WriteLine($"Could not upload {objectName} to {bucketName}.");
                return "";
            }

        }
    }
}