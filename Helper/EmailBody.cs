namespace AttendaceManagementSystemWebAPI.Helper
{
    public static class EmailBody
    {
        public static string ResetEmailBody(string email, string emailToken) 
        {
            return $@"<html>
            <head>
            </head>
            <body >
            <div >
            <div>
            <div>
            <h1>Reset your Password</h1>
            <hr>
            <p>Your receiving this email because you requested a password reset for your Facial Recognition Atttendance Management System Account.</p>
            <p>Please tap the link below to choose a new password</p>
            <a href=""http://localhost:4200/reset?email={email}&code={emailToken}"">Reset Password</a>
            <p>Kind Regards, <br><br>
            Alliance</p>
            </div>
            </div>
            </div>
            </body>
            </html>
            ";
        }
    

        public static string CredentialsEmailBody(string employeeIdNumber, string password)
        {
            return $@"<html>
            <head>
            </head>
            <body >
            <div >
            <div>
            <div>
            <h1>Account Credentials</h1>
            <hr>
            <p>Your receiving this email because you requested an account for the Facial Recognition Atttendance Management System.</p>
            <p>Id Number: {employeeIdNumber}</p>
            <p>Password: {password}</p>
            <p>Kind Regards, <br><br>
            Alliance</p>
            </div>
            </div>
            </div>
            </body>
            </html>
            ";
        }

        public static string CheckEmailBody()
        {
            return $@"<html>
            <head>
            </head>
            <body >
            <div >
            <div>
            <div>
            <h1>Checking Email</h1>
            <hr>
            <p>Hi, I’m just checking if this address is valid. </p>
            <p>Kind Regards, <br><br>
            Alliance</p>
            </div>
            </div>
            </div>
            </body>
            </html>
            ";
        }
    }
}
