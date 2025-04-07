HeritageAI

HeritageAI is a voice-based application that simulates historical conversations with figures like Abraham Lincoln. It could, in theory, be used to recreate any character, but the purpose of this project is to chat with interesting figure throughout history. It records a user's spoken input, transcribes the audio, sends it to OpenAI's GPT-4o for a contextual response, converts that response into speech using a custom voice via ElevenLabs (powered by Azure Speech), and plays the result back.

Features:
Voice recording via microphone using NAudio
Google Cloud Speech-to-Text API for transcription
OpenAI GPT-4o for historically accurate AI responses
ElevenLabs/Azure Text-to-Speech with custom Lincoln voice
Console interface for simple interaction and testing
Debug logging for tracing execution and issues

Setup Instructions:
Prerequisites:
    Windows OS
    .NET 9.0 SDK
    Google Cloud SDK
    Google Speech-to-Text API enabled
    OpenAI account + API key
    ElevenLabs account subscription
  
  Environment Variables
    Create and set the following system environment variables:
    GOOGLE_PROJECT_ID=<your-google-project-id> // This is your speech-to-text service
    GOOGLE_APPLICATION_CREDENTIALS=<full-path-to-your-service-account.json> // Also for speech-to-text
    OPENAI_API_KEY=<your-openai-api-key> // For creating the GPT
    ELEVENLABS_SPEECH_KEY=<your-elevenlabs-speech-key> // API key from elevenlabs
  These can be configured via System Properties > Environment Variables on Windows.

  Currently the pro

Running the Project:
  1. Clone the project and open in JetBrains Rider or your preferred IDE
  2. Ensure the above environment variables are correctly set
  3. Set config.SpeechSynthesisVoiceName in TextToSpeech.cs to use one of your ElevenLabs voice models.
  4. Build the project using .NET 9.0
  5. Run the application
  6. Press F4 to begin recording
  7. Speak into your microphone
  8. Press P to stop recording and receive a spoken response
  9. Press Q to quit

Transcribed audio is printed to the console
AI response is played back
waves/audio.wav: Your raw recorded voice
speech/audio.mp3: AI-generated response

Notes for Developers
  Voice playback uses NAudio for asynchronous streaming
  ElevenLabs configuration uses the voice model ID now instead of its name, you can find this by clicking the 'ID' button 
  Transcription uses Google Cloud Speech V2 with the "latest_long" model
  Edit the Content section of the first message in OpenAIChat.cs to customize the character.
  Don't forget to set your enviornment variables and ElevenLabs voice model

Features to be implemented:
  Simple GUI for ease of use including character personality customization
