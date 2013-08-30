package com.wsnremote;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.Calendar;
import java.util.Date;

import org.json.JSONArray;
import org.json.JSONObject;

import android.content.Context;
import android.content.Intent;
import android.graphics.SurfaceTexture;
import android.hardware.Camera;
import android.hardware.Camera.Parameters;
import android.hardware.Camera.PictureCallback;
import android.hardware.Camera.ShutterCallback;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.media.Ringtone;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Environment;
import android.os.Vibrator;
import android.util.Log;

public class Modules {

	// Class def
	String name;
	public String moduleId;
	String inputNode[];
	public String outputNode;
	String deviceId;
	String instanceId;
	public JSONObject send;

	private String box;
	private Context context;
	String inputs[];
	SensorManager senMan;
	static Camera cam = null;
	SurfaceTexture surfaceTexture;

	public Modules(JSONArray objects, String moduleId, String deviceId, String instanceId, Context context) {
		try {
			this.moduleId = moduleId;
			this.context = context;
			this.deviceId = deviceId;
			this.instanceId = instanceId;
			box = "";
		} catch (Exception ex) {
			Log.d("JSON", ex.toString());
		}

		try {
			for (int i = 0; i < objects.length(); i++) {
				JSONObject obj = objects.getJSONObject(i);
				if (obj.getString("moduleId").equals(moduleId)) { // if correct module
					if (!obj.getString("deviceId").equals(deviceId)) { // if not correct device id
						return;
					}
					if (!obj.getString("instanceId").equals(instanceId)) { // if not correct instance id
						return;
					}

					name = obj.getString("name");

					inputNode = new String[obj.getJSONArray("inputs").length()];
					for (int j = 0; j < inputNode.length; j++) {
						inputNode[j] = obj.getJSONArray("inputs").getString(j);
					}
					inputs = new String[inputNode.length];

					outputNode = obj.getString("output");
					if (name.equals("Number")) {
						simpleStart(obj.getJSONArray("values").getString(0));
					} else if (name.contains("Sensor")) {
						sensorStart();
					} else if (name.equals("Camera")) {
						surfaceTexture = new SurfaceTexture(1);
					}
				}
				Log.d("JSON", box.toString());
				Log.d("JSON", objects.toString(4));
			}
		} catch (Exception e) {
			Log.d("JSON-init", e.toString());
		}

	}

	public void update(JSONObject command) {
		// wait until all inputs come in then start
		try {
			if (!command.getString("instanceId").equals(instanceId)) { // if not correct instance id
				Log.d("JSON-instance", instanceId + " : " + command.getString("instanceId"));
				return;
			}

			if (inputNode == null || inputs == null) {
				return;
			}

			for (int i = 0; i < inputNode.length; i++) {
				if (command.getString("inputModuleId").equals(inputNode[i])) {
					inputs[i] = command.getString("value");
				}
			}
		} catch (Exception ex) {
			Log.d("JSON", ex.toString());
		}

		int ready = inputs.length;
		for (String s : inputs) {
			if (s != null && !s.equals("")) {
				ready--;
			}
		}

		if (ready == 0) {
			if (name.equals("Less Than")) {
				lessThanUpdate(inputs[0], inputs[1]);
			} else if (name.equals("Vibrate")) {
				actuatorStart(inputs[0]);
			} else if (name.equals("Flashlight")) {
				lightControl(inputs[0]);
			} else if (name.equals("Camera")) {
				cameraControl(inputs[0]);
			} else if (name.equals("Ring")) {
				ring(inputs[0]);
			}

			try {
				send = new JSONObject();
				send.put("moduleId", outputNode);
				send.put("inputModuleId", moduleId);
				send.put("instanceId", command.getString("instanceId"));
				send.put("value", box);
			} catch (Exception ex) {
				Log.d("JSON", ex.toString());
			}
		}
	}

	public void simpleStart(String simple) {
		box = simple;
	}

	public void actuatorStart(String input) {
		boolean b = parseBoolean(input);

		Log.d("JSON-vibrate", b + "");

		if (b) {
			Vibrator v = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
			v.vibrate(1000);
		}
	}

	public void lightControl(String input) {
		boolean b = parseBoolean(input);
		Log.d("JSON-light", b + "");

		if (cam == null) {
			cam = Camera.open();
		}

		if (b) {
			Parameters p = cam.getParameters();
			p.setFlashMode(Parameters.FLASH_MODE_TORCH);
			cam.setParameters(p);
			cam.startPreview();
		} else {
			Parameters p = cam.getParameters();
			p.setFlashMode(Parameters.FLASH_MODE_OFF);
			cam.setParameters(p);
			cam.stopPreview();
		}
	}

	public void cameraControl(String input) {
		boolean b = parseBoolean(input);
		Log.d("JSON-cam", b + "");

		if (cam == null)
			cam = Camera.open();

		if (b) {
			try {
				cam.setPreviewTexture(surfaceTexture);
				cam.setPreviewCallback(null);
				cam.startPreview();
				cam.takePicture(shutterCallback, rawCallback, jpegCallback);
			} catch (Exception ex) {
				Log.d("json-camera", ex.toString());
			}
		}
	}

	/**
	 * Initializes module as a sensor end.
	 */
	public void sensorStart() {
		int sensorType = 0;
		if (name.equals("Light Sensor")) {
			sensorType = Sensor.TYPE_LIGHT;
		} else if (name.equals("Proximity Sensor")) {
			sensorType = Sensor.TYPE_PROXIMITY;
		} else if (name.contains("Accel Sensor")) {
			sensorType = Sensor.TYPE_ACCELEROMETER;
		}

		senMan = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
		senMan.unregisterListener(listener);
		senMan.registerListener(listener, senMan.getDefaultSensor(sensorType), SensorManager.SENSOR_DELAY_FASTEST);
		box = "0";
	}

	public void ring(String input) {
		boolean b = parseBoolean(input);

		Log.d("JSON-ring", b + "");

		if (b) {
			Uri notification = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
			Ringtone r = RingtoneManager.getRingtone(context.getApplicationContext(), notification);
			r.play();
		}

	}

	public void getTime() {
		Calendar c = Calendar.getInstance();
		int hours = c.get(Calendar.HOUR_OF_DAY);
		int minutes = c.get(Calendar.MINUTE);
		int seconds = c.get(Calendar.SECOND);
		box = hours + "" + minutes + "" + seconds;
	}

	public void lessThanUpdate(String input1, String input2) {
		double x = 0, y = 0;
		try {
			x = Double.parseDouble(input1);
			y = Double.parseDouble(input2);
		} catch (Exception ex) {
			Log.d("JSON-lessthan", ex.toString());
		}
		box = String.valueOf(x < y);
	}

	public void stop() {
		if (cam != null) {
			cam.release();
			cam = null;
		}
		if (senMan != null && listener != null) {
			senMan.unregisterListener(listener);
		}
	}

	public boolean parseBoolean(String b) {
		if (b.equals("1")) {
			return true;
		}
		if (b.equals("0")) {
			return false;
		}
		try {
			return Boolean.parseBoolean(b);
		} catch (Exception ex) {
			return false;
		}
	}

	SensorEventListener listener = new SensorEventListener() {
		@Override
		public void onSensorChanged(SensorEvent event) {
			Sensor sensor = event.sensor;
			if (sensor.getType() == Sensor.TYPE_LIGHT) {
				box = event.values[0] + "";
			} else if (sensor.getType() == Sensor.TYPE_PROXIMITY) {
				box = event.values[0] + "";
			} else if (sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
				if (name.contains("Gx")) {
					box = event.values[0] + "";
				} else if (name.contains("Gy")) {
					box = event.values[1] + "";
				} else {
					box = event.values[2] + "";
				}
			}
			Log.d("JSON-sensors", box.toString());
		}

		@Override
		public void onAccuracyChanged(Sensor sensor, int accuracy) {
			// unimplemented
		}
	};

	ShutterCallback shutterCallback = new ShutterCallback() {
		public void onShutter() {
			Log.d("json-shutter", "onShutter'd");
		}
	};

	/** Handles data for raw picture */
	PictureCallback rawCallback = new PictureCallback() {
		public void onPictureTaken(byte[] data, Camera camera) {
			Log.d("json-rawpic", "onPictureTaken - raw");
		}
	};

	PictureCallback jpegCallback = new PictureCallback() {
		public void onPictureTaken(byte[] data, Camera camera) {
			FileOutputStream outStream = null;
			try {
				surfaceTexture.release();
				surfaceTexture = new SurfaceTexture(1);

				File mediaStorageDir = new File(Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES), "MugShots");
				if (!mediaStorageDir.exists())
					mediaStorageDir.mkdirs();
				String timeStamp = new Date().getTime() + "";
				File mediaFile = new File(mediaStorageDir.getPath() + File.separator + "IMG_" + timeStamp + ".jpg");
				outStream = new FileOutputStream(mediaFile);
				outStream.write(data);
				outStream.close();
				context.sendBroadcast(new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE, Uri.fromFile(mediaFile)));
			} catch (FileNotFoundException e) {
				e.printStackTrace();
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
	};
}
