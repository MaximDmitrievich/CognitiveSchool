using System.IO;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;

namespace CognitiveSchool
{
    class EmotionControl
    {
        private EmotionServiceClient _emotionServiceClient;

        private Emotion[] _emotions;

        private bool _isEmotion;

        private readonly string _emotionAPISubscriptionKey = "";

        public Emotion[] Emotions
        {
            private set { _emotions = value; }
            get { return _emotions; }
        }

        public bool IsEmotion
        {
            private set { _isEmotion = value; }
            get { return _isEmotion; }
        }

        public EmotionControl()
        {
            _emotionServiceClient = new EmotionServiceClient(_emotionAPISubscriptionKey);
            _isEmotion = false;
        }

        public async Task StartRecognize(Stream file)
        {
            try
            {
                _emotions = await _emotionServiceClient.RecognizeAsync(file);
                if (_emotions.Length > 0)
                {
                    _isEmotion = true;
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Quantity of Transaction per minute is over. Please, wait a minute");
            }
            
        }


    }
}
