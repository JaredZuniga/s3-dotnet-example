using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;
using System.Diagnostics;

namespace AWS
{
    /// <summary>
    /// Class to load AWS profiles from the current machine.
    /// </summary>
    public class CredentialsLoader
    {
        /// <summary>
        /// Gets the AWSCredentials object from the machine credentials configured.
        /// <para>If the machine has a AWS_PROFILE configured try to load the 
        /// profile and determine if its a SSO profile.</para>
        /// <para>If the machine hasn't environment variable AWS_PROFILE configured
        /// the default determined by the AWS SDK is returned with possible
        /// exception at runtime.</para>
        /// </summary>
        /// <returns>AWSCredentials</returns>
        public static AWSCredentials GetMachineCredentials()
        {
            const string environmetNameAwsProfile = "AWS_PROFILE";
            string? awsProfile = Environment.GetEnvironmentVariable(environmetNameAwsProfile);

            if(string.IsNullOrEmpty(awsProfile))
            {
                return FallbackCredentialsFactory.GetCredentials();
            }

            var chain = new CredentialProfileStoreChain();

            if (!chain.TryGetAWSCredentials(awsProfile, out AWSCredentials credentials))
            {
                return FallbackCredentialsFactory.GetCredentials();
            }

            if (credentials is SSOAWSCredentials ssoCredentials)
            {
                const string clientName = "session-csharp";
                ssoCredentials.Options.ClientName = clientName;
                ssoCredentials.Options.SsoVerificationCallback = args =>
                {
                    // Launch a browser window that prompts the SSO user to complete an SSO sign-in.
                    // This method is only invoked if the session doesn't already have a valid SSO token.
                    // NOTE: Process.Start might not support launching a browser on macOS or Linux. If not,
                    //       use an appropriate mechanism on those systems instead.
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = args.VerificationUriComplete,
                        UseShellExecute = true
                    });
                };
                return ssoCredentials;
            }

            return FallbackCredentialsFactory.GetCredentials();
        }
    }
}