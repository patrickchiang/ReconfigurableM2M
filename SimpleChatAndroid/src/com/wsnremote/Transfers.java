package com.wsnremote;

import java.util.Arrays;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONObject;

import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

import com.intel.stc.utility.StcUser;

public class Transfers extends AbstractServiceUsingActivity {

	public static String id, otherId;
	public static boolean running;
	private static boolean connected;
	private String[] capSelected;
	private String location;

	private Modules mods[];
	JSONArray deployment;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.transfer);
		running = true;

		Bundle b = getIntent().getExtras();
		capSelected = b.getStringArray("selected");
		location = b.getString("location");
	}

	@Override
	protected void onStop() {
		super.onStop();
		running = false;
	}

	protected void onPause() {
		super.onPause();
		if (mods == null)
			return;
		for (Modules m : mods) {
			if (m != null)
				m.stop();
		}
	}

	public void respondCapabilities() {
		try {
			JSONObject cap = new JSONObject();
			JSONArray capabilities = new JSONArray();

			cap.put("id", id);

			for (String capability : capSelected) {
				capabilities.put(capability);
			}
			cap.put("location", location);

			cap.put("capabilities", capabilities);

			sendData("c" + cap.toString() + "\0");
			Log.d("JSON", cap.toString());
		} catch (Exception ex) {
			Log.d("JSON", "Did not respond to capabilities query.");
		}
	}

	public void connect(String userId) {
		if (userId == null)
			return;

		try {
			List<StcUser> userList = chatService.getSTCLib().getUserListWithoutAvatar();
			for (StcUser user : userList) {
				if (user.getUserName().equals(userId)) {
					Log.d("JSON-connect", chatService.inviteUser(user) + "");
				}
			}
		} catch (Exception ex) {
			Log.d("JSON-connect", ex.toString());
		}
	}

	public void sendData(String data) {
		if (chatService != null) {
			chatService.sendString(data);
			Log.d("JSON-snd", data);
		} else
			Log.e("Error", "chatService is gone for some reasons.");
	}

	@Override
	public void onBackPressed() {
		this.finish();
		finish();
		doStopService();
		super.onBackPressed();
	}

	@Override
	public void lineReceived(int line) {
		myHandler.post(new Runnable() {
			public void run() {
				TextView debug = (TextView) findViewById(R.id.debugField);

				String cmd = "";
				List<String> lines = chatService.getLines();
				for (String str : lines) {
					debug.setText(str);
					cmd = str;
				}
				Log.d("JSON-rcv", Arrays.toString(lines.toArray()));

				if (cmd.startsWith("u")) {
					updated(cmd);
				} else if (cmd.startsWith("d")) {
					deployed(cmd);
				}
			}
		});
	}

	public void updated(String cmd) {
		cmd = cmd.substring(1);
		JSONObject heartbeat = null;
		Log.d("JSON-updated", cmd);
		try {
			heartbeat = new JSONObject(cmd);
			for (Modules m : mods) {
				if (heartbeat.getString("moduleId").equals(m.moduleId)) {
					// This is the module the heartbeat is for
					m.update(heartbeat);

					// value to send to next module
					if (m.send != null) {
						heartbeat = m.send;
						m.send = null;

						nextStep(heartbeat);
					}
					return;
				}
			}
		} catch (Exception ex) {
			Log.e("JSON-updated", "bad data?", ex);
		}
	}

	public void nextStep(JSONObject heartbeat) {
		try {
			String nextModule = heartbeat.getString("moduleId");
			String nextDevice = "";

			for (int i = 0; i < deployment.length(); i++) {
				if (deployment.getJSONObject(i).getString("moduleId").equals(nextModule)) {
					nextDevice = deployment.getJSONObject(i).getString("deviceId");
				}
			}

			Log.d("JSON-next", "Next Device: " + nextDevice);
			if (nextDevice.equals(id)) {
				updated("u" + heartbeat.toString());
			} else {
				connect(nextDevice);
				connected = false;
				while (!connected) {
					Thread.sleep(100);
				}
				sendData("u" + heartbeat.toString());
			}
		} catch (Exception ex) {
			Log.d("JSON-connect", ex.toString());
		}
	}

	public void deployed(String cmd) {
		cmd = cmd.substring(1);
		try {
			deployment = new JSONArray(cmd);
			mods = new Modules[deployment.length()];
			for (int i = 0; i < deployment.length(); i++) {
				Modules m = new Modules(deployment, i + "", id, "0", Transfers.this);
				mods[i] = m;
			}
		} catch (Exception ex) {
			Log.d("JSON-deployed", "bad data received: " + cmd);
		}
	}

	/**
	 * Starts everytime app starts/restarts
	 */
	@Override
	protected void onStcLibPrepared() {
		myHandler.post(new Runnable() {
			public void run() {
				try {
					id = chatService.getSTCLib().queryLocalUser().getUserName();
					otherId = chatService.getSTCLib().getUserListWithoutAvatar().get(0).getUserName();
				} catch (Exception ex) {
					Log.d("JSON", ex.toString());
				}
				Button simulateBtn = (Button) findViewById(R.id.simulate);
				simulateBtn.setOnClickListener(new OnClickListener() {

					@Override
					public void onClick(View v) {

						JSONArray json = new JSONArray();
						try {
							JSONObject testField = new JSONObject();

							JSONArray inputs = new JSONArray();
							JSONArray values = new JSONArray();

							values.put("2");

							testField.put("name", "Number");
							testField.put("moduleId", "1");
							testField.put("instanceId", "0");
							testField.put("deviceId", id);
							testField.put("inputs", inputs);
							testField.put("output", "2");
							testField.put("values", values);

							json.put(testField);
						} catch (Exception ex) {
							Log.d("JSON", ex.toString());
						}

						try {
							JSONObject testField2 = new JSONObject();

							JSONArray inputs2 = new JSONArray();
							JSONArray values2 = new JSONArray();

							values2.put("1");

							testField2.put("name", "Number");
							testField2.put("moduleId", "0");
							testField2.put("instanceId", "0");
							testField2.put("deviceId", id);
							testField2.put("inputs", inputs2);
							testField2.put("output", "2");
							testField2.put("values", values2);

							json.put(testField2);
						} catch (Exception ex) {
							Log.d("JSON", ex.toString());
						}

						try {
							JSONObject testField3 = new JSONObject();

							JSONArray inputs3 = new JSONArray();
							JSONArray values3 = new JSONArray();

							inputs3.put("0");
							inputs3.put("1");

							testField3.put("name", "Less Than");
							testField3.put("moduleId", "2");
							testField3.put("instanceId", "0");
							testField3.put("deviceId", otherId);
							testField3.put("inputs", inputs3);
							testField3.put("output", "3");
							testField3.put("values", values3);

							json.put(testField3);
						} catch (Exception ex) {
							Log.d("JSON", ex.toString());
						}

						try {
							JSONObject testField4 = new JSONObject();

							JSONArray inputs4 = new JSONArray();
							JSONArray values4 = new JSONArray();

							inputs4.put("2");

							testField4.put("name", "Ring");
							testField4.put("moduleId", "3");
							testField4.put("instanceId", "0");
							testField4.put("deviceId", id);
							testField4.put("inputs", inputs4);
							testField4.put("output", "");
							testField4.put("values", values4);

							json.put(testField4);
						} catch (Exception ex) {
							Log.d("JSON", ex.toString());
						}

						Modules m0 = new Modules(json, "0", id, "0", Transfers.this);
						Modules m1 = new Modules(json, "1", id, "0", Transfers.this);
						Modules m2 = new Modules(json, "2", id, "0", Transfers.this);
						Modules m3 = new Modules(json, "3", id, "0", Transfers.this);

						mods = new Modules[] { m0, m1, m2, m3 };

						sendData("d" + json.toString());
						deployed("d" + json.toString());
						try {
							Log.d("JSON-deploy", json.toString(4));
						} catch (Exception ex) {
						}
					}
				});

				Button sendBtn = (Button) findViewById(R.id.sendBtn);
				sendBtn.setOnClickListener(new OnClickListener() {
					public void onClick(View v) {
						try {
							JSONObject command = new JSONObject();
							command.put("moduleId", "0");
							command.put("inputModuleId", "0");
							command.put("instanceId", "0");
							command.put("value", "2");

							nextStep(command);

							JSONObject command2 = new JSONObject();
							command2.put("moduleId", "1");
							command2.put("inputModuleId", "0");
							command2.put("instanceId", "0");
							command2.put("value", "5");

							nextStep(command2);
						} catch (Exception ex) {
							Log.d("JSON-test", ex.toString());
						}
					}
				});

				myHandler.postDelayed(new Runnable() {
					public void run() {
						try {
							respondCapabilities();
						} catch (Exception ex) {
							Log.d("JSON-thread", ex.toString());
						}
					}
				}, 3000);
			}
		});
	}

	@Override
	public void userListChanged() {
		// unimplemented
	}

	@Override
	public void localSessionChanged() {
		// unimplemented
	}

	@Override
	public void connected(boolean didConnect) {
		//Log.d("JSON-connected", "Connected? " + didConnect);
		connected = didConnect;
	}

	@Override
	public void remoteDisconnect() {

	}
}
