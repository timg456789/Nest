# Nest Camera Status Check

Nest has camera offline alerts in their app. I unplug the camera and I get a notification about 10 minutes later. Here are my issues with it:

1. 10 Minutes is a long time to be alerted. I should be alerted within 1 minute and an absolute max of two minutes: 1 minute for function to run and 1 minute for alarm to go off.
2. Do I get an alert if the camera has its streaming functionality turned off in the app itself? I wouldn't expect that to be the default, but I want to know about it. I want my intent to be evaluated not simply a complete failure of the camera system.

I'm having some issues with false alarms. I'm sporadically seeing non-json responses from the API's. I'll add logging to see what's going on then retries when I've evaluated the situation. Until the alarm is reliable I can't measure the time to alarm, but I exect it to be around 1 minute and 30 seconds.

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
        "function-timeout"     : 60,
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
        Send message to topic "NestAlarm" (EMAIL@yahoo.com)
        Send message to topic "NestAlarm"
    In OK:
        Send message to topic "NestAlarm" (EMAIL@yahoo.com)
        Send message to topic "NestAlarm"

