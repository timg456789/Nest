# Nest Camera Status Check

## Snapshot Image Limits

[10/minute for cameras that have a Nest Aware subscription](https://developers.nest.com/documentation/cloud/camera-guide)

Here's what I need to do to get 10 images a minute. The lowest time allowed by a CloudWatch scheduled event is one minute. I need one second. This requires a constantly running task, which isn't suited for AWS Lambda.

    TimeSpan waitTime = 1s;
    TimeSpan currentWaitTime = 0s;
    TimeSpan waitTimeReset = 60s;
    
	while (true)
	{
		foreach(setting in settings)
		{
			currentWaitTime += waitTime;
			if (currentWaitTime == setting.frequency)
			{
				setting.Lambda.InvokeAsync();
			}
		}
		Thread.Sleep(waitTime);
		if (waitTime == waitTimeReset)
		{
			waitTime = 0s;
		}
	}

	new Settings
	{
		TimeSpan Frequency = 6s;
		AmazonLambdaClient Lambda = new AmazonLambdaClient()
	}

Finally I have beaten the one minute cron job limit for Linux/Lambda. Which is a step ahead of Windows 5 minute limit for scheduled tasks.

This seems pretty nifty actually and should be in a project of its own. Or just bundle it, either way the biggest concern is cost... Not sure what to do there.

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
        "function-timeout"     : 90,
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

## Nest Response

    {
    	"devices": {
    		"cameras": {
    			"string": {
    				"name": "Living Room",
    				"software_version": "string",
    				"where_id": "string",
    				"device_id": "string",
    				"structure_id": "string",
    				"is_online": true,
    				"is_streaming": true,
    				"is_audio_input_enabled": true,
    				"last_is_online_change": "2017-11-14T05:17:24.000Z",
    				"is_video_history_enabled": true,
    				"is_public_share_enabled": false,
    				"activity_zones": [{
    					"name": "Door",
    					"id": 296149
    				}],
    				"last_event": {
    					"has_sound": false,
    					"has_motion": true,
    					"has_person": true,
    					"start_time": "2017-11-22T05:42:28.360Z",
    					"end_time": "2017-11-22T05:42:33.365Z",
    					"urls_expire_time": "2017-12-02T05:42:28.360Z",
    					"web_url": "string",
    					"app_url": "string"
    					"image_url": "string",
    					"animated_image_url": "string"
    				},
    				"where_name": "Living Room",
    				"name_long": "Living Room Camera",
    				"web_url": "string",
    				"app_url": "string",
    				"snapshot_url": "string"
    			}
    		}
    	},
    	"structures": {
    		"string": {
    			"name": "Home",
    			"country_code": "US",
    			"time_zone": "America/New_York",
    			"away": "home",
    			"structure_id": "string",
    			"wheres": {
    				"string": {
    					"where_id": "string",
    					"name": "<null>"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Backyard"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Basement"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Bedroom"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Den"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Dining Room"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Downstairs"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Driveway"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Entryway"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Family Room"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Front Yard"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Guest House"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Guest Room"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Hallway"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Kids Room"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Kitchen"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Living Room"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Master Bedroom"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Office"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Outside"
    				},
    				"string": {
    					"where_id": "string",
    					"name": "Upstairs"
    				}
    			},
    			"cameras": ["string"]
    		}
    	},
    	"metadata": {
    		"access_token": "string",
    		"client_version": 1,
    		"user_id": "string"
    	}
    }
