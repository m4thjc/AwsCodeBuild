using Amazon.Lambda.Core;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using AWSLambdaTwo.Services;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambdaTwo;

public class Function
{
    //private readonly IEmailService _emailService;

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string FunctionHandler(User input, ILambdaContext context)
    {
        //db with emails
        string connectionString = "Server=database-test-emails.ccsuaworum1w.us-east-1.rds.amazonaws.com,1433;Database=EmailData;User ID=admin;Password=Sultan1!Halo;";

        var toReturn = input.Name.ToUpper();

        var dbConnectionSucceeded = testDbConnection(connectionString);

        
        try
        {
            var emailInfo = GetEmailInfo(connectionString);
            sendEmail(input);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        

        return toReturn;
    }

    private List<emailRow> GetEmailInfo(string connectionString)
    {
        List<emailRow > emails = new List<emailRow>();
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("GetEmailAddressInfo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters if needed
                    //command.Parameters.AddWithValue("@ParameterName", parameterValue);

                    // Execute the stored procedure
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Process the results
                        while (reader.Read())
                        {
                            var thisRow = new emailRow()
                            {
                                EmailId = reader.GetInt64(0),
                                Email = reader.GetString(1),
                            };
                            //// Access the columns by name or index
                            //var EmailId = reader.GetInt64(0);
                            //var email = reader.GetString(1);
                            emails.Add(thisRow);
                            // Process the values as needed
                            Console.WriteLine($"Column1: {thisRow.EmailId}, Column2: {thisRow.Email}");
                            LambdaLogger.Log($"Id: {thisRow.EmailId}, Name: {thisRow.Email}");
                        }
                    }
                }
            }
            Console.WriteLine("Stored procedure executed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }

        return emails;
    }

    public class emailRow
    {
        public long EmailId { get; set; }
        public string Email { get; set; } = "";
    }

    private bool testDbConnection(string connectionString)
    {
        bool connectionSucceeded;

        try
        {
            using SqlConnection connection = new SqlConnection(connectionString);

            // Open the connection
            connection.Open();

            // Perform database operations...

            // Close the connection
            connection.Close();

            connectionSucceeded = true;
        }
        catch  {
            connectionSucceeded = false;
        }



        return connectionSucceeded;
    }

    private void sendEmail(User user)
    {
        string message;

//        static readonly string textBody = "Amazon SES Test (.NET)\r\n"
//                                + "This email was sent through Amazon SES "
//                                + "using the AWS SDK for .NET.";

//        // The HTML body of the email.
//        static readonly string htmlBody = @"<html>
//<head></head>
//<body>
//  <h1>Amazon SES Test (AWS SDK for .NET)</h1>
//  <p>This email was sent with
//    <a href='https://aws.amazon.com/ses/'>Amazon SES</a> using the
//    <a href='https://aws.amazon.com/sdk-for-net/'>
//      AWS SDK for .NET</a>.</p>
//</body>
//</html>";

//        using (var client = new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.USEast1))
//        {
            //var sendRequest = new SendEmailRequest
            //{
            //    Source = "john.mathias3@gmail.com",
            //    Destination = new Destination
            //    {
            //        ToAddresses =
            //        new List<string> { "john.mathias3@gmail.com" }
            //    },
            //    Message = new Message
            //    {
            //        Subject = new Content("email subject from John"),
            //        Body = new Body
            //        {
            //            Html = new Content
            //            {
            //                Charset = "UTF-8",
            //                Data = htmlBody
            //            },
            //            Text = new Content
            //            {
            //                Charset = "UTF-8",
            //                Data = textBody
            //            }
            //        }
            //    },
            //    // If you are not using a configuration set, comment
            //    // or remove the following line 
            //    ConfigurationSetName = configSet
            //};

        message = $@"<p>Something worked if you got this, hopefully there is an email after this <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{user.Name}</code></p>";

        var _emailService = new EmailService();
        _emailService.Send(
            to: "john.mathias3@gmail.com",
            subject: "Sign-up Verification API - Verify Email",
            html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
        );
    }
}
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

