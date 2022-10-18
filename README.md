# Vocal.Net

Vocal.Net is a vocal assistant created with .NET 6 and C# . Works on Ubuntu and French language.

Vocal.Net is divided into 3 parts

## Vocal.Intents

An Intent Recognizer based on the sentence said by the user. It uses ML.NET.

You can write all your intents in Data/intents folder. An example is available here

## Vocal.Tts

A wrapper for Text To Speech Engine eSpeak. If you're using Windows, you can delete this part and use System.Speech instead.

## Vocal.Main

Contains the Speech To Text engine Vosk and PortAudioSharp. 

For other languages, feel free to delete all folders and file in the model folder and choose another model here : https://alphacephei.com/vosk/models