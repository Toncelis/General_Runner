using UnityEngine;

namespace Services {
    public class MessagesService : Service {
        private MessageUI _messageUI;
        
        public override void SetupService(ISourceOfServiceDependencies source) {
            _messageUI = source.messageUI;
        }

        public void ShowMessage(string text, Sprite speaker) {
            _messageUI.DisplayMessage(text, speaker);
        }
    }
}