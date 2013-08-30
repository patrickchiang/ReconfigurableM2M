INTRODUCTION

SimpleChat is an example application for Android that interoperates
with the SimpleChat application for Windows. It is intended to
demonstrate how to create a CCF based Android application. Where
SimpleChat and the current CCF documentation differ, SimpleChat takes
precedence.

SimpleChat has not yet been through serious testing and has only been
tested against the Windows SimpleChat client. Cases such as "invitation
when already connected" have also not yet been tested. Various race
conditions with user selection for outgoing invitation competing with
incoming invitation and connection have been coded for, but not
tested.

BUILDING

The following steps assume you already have installed Eclipse and the 
Android Platform SDK (http://developer.android.com/sdk/index.html) with 
API 15 or above.

1) 	In Eclipse, go to File -> New -> Project...
2) 	In the wizard, select Android -> Android Project from Existing Code.
3) 	Click Next.
4) 	Click Browse...
5) 	Select the folder that contains the SimpleChatAndroid project
	(ex. C:\SDK\Samples\SimpleChatAndroid).
6) 	Click Finish.  SimpleChatAndroid should display in Package Explorer
	as SelectUserActivity.  Rename this to SimpleChatAndroid if desired.
7) 	Open Properties of SimpleChatAndroid.
8) 	Go to Java Build Path and remove stclib.jar and stclibcc.jar from 
	Libraries.
9) 	Go to Android and verify the "Project Build Target" is API Level 15 
	or greater.
10)	Close the properties window by clicking OK.
11) Go to File -> New -> Project...
12) In the wizard, select Android -> Android Project from Existing Code.
13) Click Next.
14) Click Browse...
15) Select the folder that contains the inproc_lib project (ex.
	C:\SDK\Samples\inproc_lib).
16) Click Finish.  inproc_lib should display in the Package Explorer as
	StarterActivity.  Rename this to inproc_lib if desired.
17) Open Properties of inproc_lib.
18) Go to Android and verify the "Project Build Target" is API Level 8
	or greater.  Also, make sure the checkbox "Is Library" is checked.
19) Go to Java Build Path and remove stclib.jar from Libraries.  Click 
	"Add External JARs...".  Browse and select the location of stclibcc.jar
	(ex. C:\SDK\Lib\stclibcc.jar).
20)	Next, Click "Add External JARs..." again.  Browse and select the
	location of android-support-v4.jar 
	(ex. C:\Program Files (x86)\Android\android-sdk\extras\android\support\v4).
	If this JAR is missing, open the Android SDK Manager and download it
	under Extras -> Android Support Library.
21) Go to the "Order and Export" tab.  Check stclibcc.jar and
	android-support-v4.jar.  Click OK.
22) Open Properties of SimpleChatAndroid.  Go to Android and add a reference
	to inproc_lib.
23) Build inproc_lib.  Resolve any errors that may occur.
24) Build SimpleChatAndroid.  Resolve any errors that may occur.

INSTALLING THE PLATFORM

The CC3.apk must be installed before
SimpleChat is installed.

THINGS TO NOT WORRY ABOUT

The caught ClassNotFoundException that causes the 
Android debugger to halt is fine. Simply continue.

THEORY OF OPERATION:

Almost all the work is done in SimpleChatService. This is where the
application lifecycle is handled and where all events are sent out to
the UI. Critical registration information is in three places:
SimpleChatService, SimpleChatRegisterApp and the manifest itself.

FILES TO EXAMINE:

If you are only going to look at a subset of the sample app, the classes
to examine and understand are:

SimpleChatService
UserAdapter
SimpleChatRegisterApp
AndroidManifest.xml

FILE INVENTORY:

AbstractServiceUsingActivity: Convenient superclass for dealing with
service binding

ChatActivity: The UI for chat

ChatAdapter: More UI for chat

IServiceIOListener: Listener interface used by the activities to
receive chat events

ISimpleChatEventListener: Event interface used by the UI to know when
state has changed

ReadEngine: Reads input from the other side

SelectUserActivity: UI for user selection

SimpleChatError: Error class used in the few places where something
unexpected and unrecoverable has occurred

SimpleChatRegisterApp: App registration information

SimpleChatService: Where the majority of the CCF integration happens

UserAdapter: Deals with StcUser objects and decides which get
displayed

WriteEngine: Writes output to the other side

Each of the files has noted in the first code comment whether there
is CCF specific information in it.

OUTGOING INVITATION FLOW

The Android main activity is the SelectUserActivity. In a standalone
start (started from the debugger or by the user clicking the icon) the
SelectUserActivity will run a user selection list. The UserAdapter
filters discovered users to those that are available (have not aged
out due to missed discoveries) and that have the SimpleChat
application installed. Once a user is selected, the service is
instructed to invite the user. On connection, the SelectUserActivity
invokes the ChatActivity.

INCOMING INVITATION PRESTARTED FLOW

If SimpleChat is running and sitting at the invitation screen when an
invitation comes in, then the service will receive the invitation in
SimpleChatService.connectionRequest(). SimpleChat automatically
accepts any incoming invitation, so it will accept the invite and
notify the SelectUserActivity when the connection has been made. On
connection the SelectUserActivity will invoke the ChatActivity.

INCOMING INVITATION, NOT PRESTARTED FLOW

If the platform is not already running (some CCF app must have been
previously run) and a SimpleChat invitation is received then the
SimpleChatRegisterApp.respondToInvite method is invoked. This method's
default implementation causes a notification to be created and
presented to the user.

The notification itself will send the launch activitiy intent to the
client bundle to the launch application appropriately.

From this point operation is the same as before. The service gets
notified of the connection. The service notifies the
SelectUserActivity. SelectUserActivity invokes the ChatActivity.

