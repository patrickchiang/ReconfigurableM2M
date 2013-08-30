/*
Copyright (c) 2011-2013, Intel Corporation

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

 * Redistributions of source code must retain the above copyright notice,
      this list of conditions and the following disclaimer.

 * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and/or other materials provided with the distribution.

 * Neither the name of Intel Corporation nor the names of its contributors
      may be used to endorse or promote products derived from this software
      without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
OF THE POSSIBILITY OF SUCH DAMAGE.
 */

package com.wsnremote;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.Date;
import java.util.List;

import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.SurfaceTexture;
import android.hardware.Camera;
import android.hardware.Camera.CameraInfo;
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
import android.os.Bundle;
import android.os.Environment;
import android.os.Vibrator;
import android.util.Log;
import android.view.View;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.TextView;

/***
 * Activity that handles the listview for received text, the edit field for text
 * to send and the send button.
 * <p>
 * There is nothing c3 specific here.
 */
public class ChatActivity extends AbstractServiceUsingActivity {

	List<String> cmd;
	static Camera cam = null;
	int sensorType;
	private static final String TAG = "CameraCode";
	SurfaceTexture surfaceTexture;
	int cameraId;
	SensorManager sensorManager;
	double previousReading = 0;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.chat);

		// Populate sensors list
		Spinner sensors = (Spinner) findViewById(R.id.sensorList);
		ArrayAdapter<CharSequence> senAdapter = ArrayAdapter
				.createFromResource(this, R.array.sensors,
						android.R.layout.simple_spinner_item);
		senAdapter
				.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
		sensors.setAdapter(senAdapter);
		sensors.setOnItemSelectedListener(new OnItemSelectedListener() {
			@Override
			public void onItemSelected(AdapterView<?> parentView,
					View selectedItemView, int position, long id) {
				initSensors();
			}

			@Override
			public void onNothingSelected(AdapterView<?> parentView) {
			}
		});

		// Populate actuators list
		Spinner actuators = (Spinner) findViewById(R.id.actuatorList);
		ArrayAdapter<CharSequence> actAdapter = ArrayAdapter
				.createFromResource(this, R.array.actuators,
						android.R.layout.simple_spinner_item);
		actAdapter
				.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
		actuators.setAdapter(actAdapter);
		actuators.setOnItemSelectedListener(new OnItemSelectedListener() {
			@Override
			public void onItemSelected(AdapterView<?> parentView,
					View selectedItemView, int position, long id) {
				initSensors();
			}

			@Override
			public void onNothingSelected(AdapterView<?> parentView) {
			}
		});

		Button b = (Button) findViewById(R.id.sendBtn);
		b.setOnClickListener(new View.OnClickListener() {
			public void onClick(View arg0) {
				sensorManager.unregisterListener(proxSensorEventListener);
				sensorManager.unregisterListener(accelSensorEventListener);
				sensorManager.unregisterListener(lightSensorEventListener);
				finish();
				System.exit(0);
			}
		});

		findFrontCamera();
		surfaceTexture = new SurfaceTexture(1);
	}

	private void initSensors() {
		sensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
		Sensor lightSensor = sensorManager.getDefaultSensor(Sensor.TYPE_LIGHT);
		Sensor proxSensor = sensorManager
				.getDefaultSensor(Sensor.TYPE_PROXIMITY);
		Sensor accelSensor = sensorManager
				.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);

		Spinner sensors = (Spinner) findViewById(R.id.sensorList);
		String sensor = ((String) sensors.getSelectedItem()).trim();

		if (sensor.equals("Light Sensor")) {
			sensorType = Sensor.TYPE_LIGHT;
			sensorManager.registerListener(lightSensorEventListener,
					lightSensor, SensorManager.SENSOR_DELAY_FASTEST);
			sensorManager.unregisterListener(proxSensorEventListener);
			sensorManager.unregisterListener(accelSensorEventListener);
		} else if (sensor.equals("Proximity Sensor")) {
			sensorType = Sensor.TYPE_PROXIMITY;
			sensorManager.registerListener(proxSensorEventListener, proxSensor,
					SensorManager.SENSOR_DELAY_FASTEST);
			sensorManager.unregisterListener(lightSensorEventListener);
			sensorManager.unregisterListener(accelSensorEventListener);
		} else if (sensor.equals("Accelerometer")) {
			sensorType = Sensor.TYPE_ACCELEROMETER;
			sensorManager.registerListener(accelSensorEventListener,
					accelSensor, SensorManager.SENSOR_DELAY_FASTEST);
			sensorManager.unregisterListener(lightSensorEventListener);
			sensorManager.unregisterListener(proxSensorEventListener);
		}
	}

	@Override
	public void onBackPressed() {
		finish();
		doStopService();
		super.onBackPressed();
	}

	@Override
	public void lineReceived(int line) {
		myHandler.post(new Runnable() {
			public void run() {
				cmd = chatService.getLines();
				TextView debug = (TextView) findViewById(R.id.debugField);
				debug.setText(cmd.get(cmd.size() - 1));

				String actuate = cmd.get(cmd.size() - 1).trim();

				if (getPackageManager().hasSystemFeature(
						PackageManager.FEATURE_CAMERA_FLASH)) {
					if (actuate.equals("Light")) {
						try {
							cam = Camera.open();
							Parameters p = cam.getParameters();
							p.setFlashMode(Parameters.FLASH_MODE_TORCH);
							cam.setParameters(p);
							cam.startPreview();
						} catch (RuntimeException ex) {
							Log.e("Error", ex.toString());
						} // camera used too often?
					}
				}
				if (actuate.equals("Vibrate")) {
					Vibrator v = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
					v.vibrate(1000);
				}
				if (actuate.equals("Camera")) {
					try {
						cam = Camera.open(cameraId);
						cam.setPreviewTexture(surfaceTexture);
						cam.setPreviewCallback(null);
						cam.startPreview();
						cam.takePicture(shutterCallback, rawCallback,
								jpegCallback);
					} catch (RuntimeException ex) {
						Log.e("Error", ex.toString());
					} catch (IOException e) {
						Log.e("Error", e.toString());
					}
				}
				if (actuate.equals("Notify")) {
					Uri notification = RingtoneManager
							.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
					Ringtone r = RingtoneManager.getRingtone(
							getApplicationContext(), notification);
					r.play();
				}
			}
		});
	}

	private void findFrontCamera() {
		cameraId = -1;
		int numberOfCameras = Camera.getNumberOfCameras();
		for (int i = 0; i < numberOfCameras; i++) {
			CameraInfo info = new CameraInfo();
			Camera.getCameraInfo(i, info);
			if (info.facing == CameraInfo.CAMERA_FACING_FRONT) {
				Log.d(TAG, "Camera found " + info.facing);
				cameraId = i;
				break;
			}
		}
	}

	@Override
	public void remoteDisconnect() {
		myHandler.post(new Runnable() {
			public void run() {
				finish();
			}
		});
	}

	public void sendData(String data) {
		if (chatService != null)
			chatService.writeString(data);
		else
			Log.e("Error", "chatService is gone for some reasons.");
	}

	@Override
	public void onStcLibPrepared() {
		myHandler.post(new Runnable() {
			public void run() {
				initSensors();
			}
		});
	}

	@Override
	public void userListChanged() {
	}

	@Override
	public void connected(boolean didConnect) {
	}

	@Override
	public void localSessionChanged() {
	}

	SensorEventListener lightSensorEventListener = new SensorEventListener() {
		@Override
		public void onAccuracyChanged(Sensor sensor, int accuracy) {

		}

		@Override
		public void onSensorChanged(SensorEvent event) {
			if (sensorType == Sensor.TYPE_LIGHT) {
				float currentReading = event.values[0];
				Spinner actuators = (Spinner) findViewById(R.id.actuatorList);
				if (currentReading >= 25.0) {
					sendData((String) actuators.getSelectedItem());
				} else {
					sendData("Nothing");
				}
				Log.i("Info", "Light: " + currentReading);
			}
		}
	};

	SensorEventListener proxSensorEventListener = new SensorEventListener() {
		@Override
		public void onAccuracyChanged(Sensor sensor, int accuracy) {

		}

		@Override
		public void onSensorChanged(SensorEvent event) {
			if (sensorType == Sensor.TYPE_PROXIMITY) {
				float currentReading = event.values[0];
				Spinner actuators = (Spinner) findViewById(R.id.actuatorList);
				if (currentReading <= 5.0) {
					sendData((String) actuators.getSelectedItem());
				} else {
					sendData("Nothing");
				}
				Log.i("Info", "Proximity: " + currentReading);
			}
		}
	};

	SensorEventListener accelSensorEventListener = new SensorEventListener() {
		@Override
		public void onAccuracyChanged(Sensor sensor, int accuracy) {

		}

		@Override
		public void onSensorChanged(SensorEvent event) {
			if (sensorType == Sensor.TYPE_ACCELEROMETER) {
				double currentReading = Math.abs(event.values[2] - previousReading);
				Spinner actuators = (Spinner) findViewById(R.id.actuatorList);
				if (currentReading >= 1.0) {
					sendData((String) actuators.getSelectedItem());
					previousReading = event.values[2];
				} else {
					sendData("Nothing");
				}
				Log.i("Info", "Accelerometer: " + currentReading);
			}
		}
	};

	ShutterCallback shutterCallback = new ShutterCallback() {
		public void onShutter() {
			Log.d(TAG, "onShutter'd");
		}
	};

	/** Handles data for raw picture */
	PictureCallback rawCallback = new PictureCallback() {
		public void onPictureTaken(byte[] data, Camera camera) {
			Log.d(TAG, "onPictureTaken - raw");
		}
	};

	/** Handles data for jpeg picture */
	PictureCallback jpegCallback = new PictureCallback() {
		public void onPictureTaken(byte[] data, Camera camera) {
			FileOutputStream outStream = null;
			
			if (camera != null) {
				cam.stopPreview();
				cam.setPreviewCallback(null);
				cam.release();
				cam = null;
			}
			
			try {
				File mediaStorageDir = new File(Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES), "MugShots");
				if(!mediaStorageDir.exists())
					mediaStorageDir.mkdirs();
				String timeStamp = new Date().getTime() + "";
				File mediaFile = new File(mediaStorageDir.getPath() + File.separator + "IMG_" + timeStamp + ".jpg");
				outStream = new FileOutputStream(mediaFile);
				outStream.write(data);
				outStream.close();
				
				sendBroadcast(new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE, Uri.fromFile(mediaFile)));
				Log.d(TAG, "onPictureTaken - wrote bytes: " + data.length);
			} catch (FileNotFoundException e) {
				e.printStackTrace();
			} catch (IOException e) {
				e.printStackTrace();
			} finally {
			}
			Log.d(TAG, "onPictureTaken - jpeg");
		}
	};

}
