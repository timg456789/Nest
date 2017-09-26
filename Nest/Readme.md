# Nest Camera Status Check

Ugh, I just realized now after unplugging the camera for an extended period of time Nest provides this functionality. I still have issues with only 30 days of data even with their subscription. I'll go after that issue next. The start is getting a snapshot every minute (although that's not too great, but it's a start at data in perpetuity)

Hah, my alarming has quicker response time being about a minute. A minute is slow as well, but that's how fast the Lambda function can be triggered by CloudWatch. I was hoping for at least 30 seconds which are the non-nest aware limits (although I don't see that being enforced, but the quality is terrible - apparently it's better if you are viewing the stream).

I'll have to measure and compare the response times between my alarming and the Nest app's. So far I see my alarm going off within a minute and Nest's at 10x that in 10 minutes. I will have to confirm tomorrow. The other question is, does the Nest app let you know when this is back on?

The awesome thing about this is, I basically have a wifi access alarm right now that is pretty darn fast. I've seen that as a stand-alone product and was thinking I would have to set that up to see if my wifi were to go down due to jamming.

1. Authenticate with an access token set in environment variables and check all camera statuses. If there any cameras offline or not streaming, the Lambda function will throw an error.

2. Trigger Lambda function every minute.

3. Check Lambda errors and require a check-in and no errors.

4. If there is no data or an error notify on status failing and again when status succeeds.

## Lambda Deploy Setup

**This is gitignored, because it contains the sensitive access_token**

    {
        "Information" : [
            "This file provides default values for the deployment wizard inside Visual Studio and the AWS Lambda commands added to the .NET Core CLI.",
            "To learn more about the Lambda commands with the .NET Core CLI execute the following command at the command line in the project root directory.",
            "dotnet lambda help",
            "All the command line options for the Lambda command can be specified in this file."
        ],
        "profile"     : "default",
        "region"      : "us-east-1",
        "configuration" : "Release",
        "framework"     : "netcoreapp1.0",
        "function-runtime" : "dotnetcore1.0",
        "function-memory-size" : 256,
        "function-timeout"     : 30,
        "function-handler"     : "Nest::Nest.Function::FunctionHandler",
        "function-name"        : "NestStatusCheck",
        "function-role"        : "arn:aws:iam::363557355695:role/lambda_exec_NestStatusCheck",
        "environment-variables" : "\"access_token\"=\"REDACTED\""
    }

## Manual Alarm Setup in AWS

    Threshold:The condition in which the alarm will go to the ALARM state.

    Errors >= 1 for 1 minute

    Actions:The actions that will occur when the alarm changes state.

    In ALARM:
        Send message to topic "NestAlarm" (timg456789@yahoo.com)
        Send message to topic "NestAlarm"
    In OK:
        Send message to topic "NestAlarm" (timg456789@yahoo.com)
        Send message to topic "NestAlarm"

