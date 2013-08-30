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
import java.util.Collections;
import java.util.List;
import java.util.UUID;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.intel.stc.utility.StcUser;

/***
 * Manages the data from the service to show the user list.
 * <p>
 * This code shows use of the StcUser. How to get the name. How to get the avatar.
 * How to get the list of applications that the user has. c3 specific content is 
 * concentrated in the two methods getView and setNewUserList.
 */
public class UserAdapter extends BaseAdapter {

	public List<StcUser> userList = new ArrayList<StcUser>();
	SimpleChatService service;
	SelectUserActivity selectActivity;

	/***
	 * Creates the view for one item in the list.
	 */
	@Override
	public View getView(int position, View convertView, ViewGroup parent) 
	{
		if (convertView == null) {
			LayoutInflater inflater = (LayoutInflater)service.getApplicationContext()
					.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			convertView = inflater.inflate(R.layout.user_row, null);
		} else
			convertView.setVisibility(View.VISIBLE);

		StcUser curUser = null;
		synchronized (userList) {
			if (position >= 0 && position < userList.size()) 
				curUser = (StcUser)getItem(position);
		}
		
		if( curUser == null ) {
			convertView.setVisibility(View.GONE);
			return convertView;
		}

		// get the avatar from the user and put it into the image view
		ImageView avatar = (ImageView)convertView.findViewById(R.id.row_userAvatar);
		if (curUser.getAvatar() == null)
			avatar.setImageResource(R.drawable.generic_avatar);
		else
			avatar.setImageBitmap(curUser.getAvatar());

		// get the name from the user and put it into the text view
		TextView userName = (TextView)convertView.findViewById(R.id.row_userName);
		userName.setText(curUser.getUserName());
		
		// get the secure code from the user and put it 
		// into the text view if the user is registered with the cloud
		if(curUser.isRegisteredWithCloud()) {
			TextView keyCode = (TextView)convertView.findViewById(R.id.row_userKey);
			keyCode.setText(String.format("[%s]", curUser.getSecurityCode()));
		}

		// setup a click handler to pass invites up to the service.
		final StcUser user = curUser;
		convertView.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				selectActivity.inviteUser(user);
			}
		});

		return convertView;
	}

	/***
	 * Receive the new list and filter it for users that are available and users
	 * that have this application.
	 * @param newList
	 */
	public void setNewUserList(List<StcUser> newList)
	{
		synchronized(userList) 
		{
			if(newList.size() == 0 )
				userList = newList;
			else
			{
				for( int i = newList.size() - 1; i >= 0 ; i-- )
				{
					StcUser user = newList.get(i);
					if( !user.isAvailable() || user.isExpired() )
					{
						// remove if the user is listed as unavailable or busy.
						newList.remove(i);
					} 
					else 
					{
						// remove if the user is listed as 
						UUID userApps[] = user.getAppList();
						boolean foundApp = false;
						for(UUID userApp : userApps)
						{
							if(userApp.toString().compareToIgnoreCase(SimpleChatRegisterApp.APP_UUID) == 0)
							{
								foundApp = true;
								break;
							}
						}
						if( !foundApp )
							newList.remove(i);
					}
				}
				userList = newList;
				Collections.sort(userList);
			}
		}
	}

	public UserAdapter(SimpleChatService service, SelectUserActivity selectActivity) {
		this.service = service;
		this.selectActivity = selectActivity;
		setNewUserList(service.getUsers());
	}

	@Override
	public int getCount() 
	{
		synchronized(userList) {
			return userList.size();
		}
	}

	@Override
	public Object getItem(int position) 
	{
		synchronized(userList) {
			if (userList != null && position < userList.size() && position >= 0)
				return userList.get(position);
			else
				return null;
		}
	}

	@Override
	public long getItemId(int position) 
	{
		return position;
	}

}