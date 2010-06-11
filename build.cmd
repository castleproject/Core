@ECHO OFF
REM ****************************************************************************
REM Copyright 2004-2010 Castle Project - http://www.castleproject.org/
REM Licensed under the Apache License, Version 2.0 (the "License");
REM you may not use this file except in compliance with the License.
REM You may obtain a copy of the License at
REM 
REM     http://www.apache.org/licenses/LICENSE-2.0
REM 
REM Unless required by applicable law or agreed to in writing, software
REM distributed under the License is distributed on an "AS IS" BASIS,
REM WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
REM See the License for the specific language governing permissions and
REM limitations under the License.
REM ****************************************************************************

IF "%1" == "" (SET Framework=NET40) ELSE (SET Framework=%1)
IF "%2" == "" (SET Target=RunAllTests) ELSE (SET Target=%2)
IF "%3" == "" (SET Configuration=Release) ELSE (SET Configuration=%3)

@call buildscripts\build.cmd %Framework% %Target% %Configuration%
