// SimpleUrlRewrite.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "SimpleUrlRewrite.h"

BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

BOOL WINAPI GetFilterVersion(PHTTP_FILTER_VERSION pVer)
{
	// our filter version
	pVer->dwFilterVersion = MAKEWORD(1,0);
	
	// What events are we interested
	pVer->dwFlags = SF_NOTIFY_ORDER_DEFAULT | 
				    SF_NOTIFY_SECURE_PORT | SF_NOTIFY_NONSECURE_PORT | 
					SF_NOTIFY_PREPROC_HEADERS;

	return TRUE;
}

//
// Simplistic approach to rewrite urls that do not use 
// a file extension.
// The best approach would be to add CoR semantics and 
// use reg ex. 
//
// Some regex links:
//   http://codeproject.com/cpp/yard-tokenizer.asp
//   http://codeproject.com/cpp/rexsearch.asp
//
// Ashamed author: hammett at gmail dot com
// Date: 01-march-2005
//
DWORD WINAPI HttpFilterProc(PHTTP_FILTER_CONTEXT pfc,
   DWORD notificationType,
   LPVOID pvNotification)
{
	if (SF_NOTIFY_PREPROC_HEADERS == notificationType)
	{
		HTTP_FILTER_PREPROC_HEADERS* headers = (HTTP_FILTER_PREPROC_HEADERS*) pvNotification;
	
		char buffer[2048];
		DWORD bufSize = 2048;

		if (!headers->GetHeader(pfc, "URL", &buffer, &bufSize))
		{
			return SF_STATUS_REQ_FINISHED_KEEP_CONN;
		}

		char* pQueryStringStart = strchr(buffer, '?');

		if (pQueryStringStart) *pQueryStringStart = '\0';

		char* pLastDir = strrchr(buffer, '/');
		
		if (!pLastDir) // No last dir?
		{
			return SF_STATUS_REQ_NEXT_NOTIFICATION;
		}

		char toAdd[12];
		memset(toAdd, 0, 12);

		bool skip = true;

		// if there is nothing forward, then _maybe_ it's and index.rails
		if ( *(pLastDir + 1) == '\0')
		{
			// Too simplist!

			strcpy(toAdd, "index.rails");
			skip = false;
		}
		else
		{
			// We restricted the search to the interval
			// of the last / and the start of the query string
			char* pFileExt = strrchr(pLastDir, '.');

			if (!pFileExt)
			{
				strcpy(toAdd, ".rails");
				skip = false;
			}
		}

		if (!skip)
		{
			char newUrl[2100];
			memset(newUrl, 0, 2100);

			DWORD newBufferLen = bufSize + strlen(toAdd);

			strncpy(newUrl, buffer, strlen(buffer));
			strcat(newUrl, toAdd);

			if(pQueryStringStart)
			{
				*pQueryStringStart = '?';
				strcat(newUrl, pQueryStringStart);
			}
			

			headers->SetHeader(pfc, "URL", newUrl);
		}
	}

	return SF_STATUS_REQ_NEXT_NOTIFICATION;
}