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

import java.util.ArrayList;

import android.app.Dialog;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.util.SparseBooleanArray;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.ListView;

import com.intel.stc.events.StcException;
import com.intel.stc.utility.StcUser;

/***
 * This activity runs a user selection screen and either waits for an incoming
 * invitation or waits for the user to select another user to invite.
 * <p>
 * There is no c3 specific code here.
 */
public class SelectUserActivity extends AbstractServiceUsingActivity implements ISimpleChatEventListener, OnClickListener {
	static final String LOGC = "sc select user";
	UserAdapter userAdapter;
	Bundle bundle;
	Dialog upDialog = null;
	ListView lview = null, capView = null;
	ArrayAdapter<String> capAdapter;
	String[] capSelected = {};
	EditText location = null;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		bundle = this.getIntent().getExtras();

		setContentView(R.layout.select);

		capView = (ListView) findViewById(R.id.capView);
		lview = (ListView) findViewById(R.id.userListView);
		location = (EditText) findViewById(R.id.location);

		doStartService();
		Transfers.running = false;

		String[] capabilities = getResources().getStringArray(R.array.capabilities);
		capAdapter = new ArrayAdapter<String>(this, android.R.layout.simple_list_item_multiple_choice, capabilities);
		capView.setChoiceMode(ListView.CHOICE_MODE_MULTIPLE);
		capView.setAdapter(capAdapter);
		for (int i = 0; i < capView.getCount(); i++) {
			capView.setItemChecked(i, true);
		}
	}

	@Override
	protected void onDestroy() {
		if (upDialog != null)
			upDialog.dismiss();
		super.onDestroy();
	}

	@Override
	public void onBackPressed() {
		Log.i(LOGC, "back pressed");
		finish();
		doStopService();
		super.onBackPressed();
	}

	public void inviteUser(StcUser user) {
		if (chatService.inviteUser(user)) {
			//upDialog = ProgressDialog.show(this, "", "Waiting for connection");
		}
	}

	// /
	// / ISimpleChatEventListener methods
	// /

	public void userListChanged() {
		myHandler.post(new Runnable() {
			public void run() {
				if (userAdapter != null) {
					Log.i(LOGC, "updating list");
					userAdapter.setNewUserList(chatService.getUsers());
					userAdapter.notifyDataSetChanged();
				}
			}
		});
	}

	private void refreshRegistration() {
	}

	@Override
	public void connected(final boolean didConnect) {
		myHandler.post(new Runnable() {
			public void run() {
				if (didConnect) {
					Log.i(LOGC, "successful connection");
					if (!Transfers.running) {
						Intent ni = new Intent(SelectUserActivity.this, Transfers.class);

						SparseBooleanArray checked = capView.getCheckedItemPositions();
						ArrayList<String> selectedItems = new ArrayList<String>();
						for (int i = 0; i < checked.size(); i++) {
							int position = checked.keyAt(i);
							if (checked.valueAt(i))
								selectedItems.add(capAdapter.getItem(position));
						}

						capSelected = new String[selectedItems.size()];
						for (int i = 0; i < selectedItems.size(); i++) {
							capSelected[i] = selectedItems.get(i);
						}

						Bundle b = new Bundle();
						b.putStringArray("selected", capSelected);
						b.putString("location", location.getText() + "");
						ni.putExtras(b);
						startActivity(ni);

						finish();
					}
				} else {
					Log.i(LOGC, "connection failed, shutting down.");
					doStopService();
					finish();
				}
			}

		});
	}

	public void lineReceived(int count) {
		Log.i(LOGC, "line received event in SelectUserActivity");
	}

	public void remoteDisconnect() {
		Log.i(LOGC, "remote disconnect event in SelectUserActivity");
	}

	@Override
	public void onStcLibPrepared() {
		userAdapter = new UserAdapter(chatService, this);
		chatService.parseInitBundle(bundle);

		myHandler.post(new Runnable() {
			public void run() {
				ListView lview = (ListView) findViewById(R.id.userListView);
				lview.setAdapter(userAdapter);

				refreshRegistration();

				String versionText = "Platform Version Unknown";
				try {
					byte[] ver = chatService.getSTCLib().queryPlatformVersion();
					versionText = String.format("Platform Version = %d.%d.%d", ver[0], ver[1], ver[2]);
				} catch (StcException e) {
				}

				setTitle("Intelligent M2M CCF " + versionText);
			}
		});
	}

	@Override
	public void onClick(View v) {
	}

	@Override
	public void localSessionChanged() {
		myHandler.post(new Runnable() {
			public void run() {
				refreshRegistration();
			}
		});
	}
}
