using System.Collections;
using DataTypes;
using DefaultNamespace.Managers;
using DefaultNamespace.Services;
using Services;
using UnityEngine;
using World.Model;

namespace World.RoomBehaviour {
    [CreateAssetMenu(menuName = "SO/RoomBehaviours/Tutorial", fileName = "TutorialBehaviour")]
    public class TutorialBehaviour : RoomScriptableBehaviour {
        [SerializeField] private CollectableTypesEnum Common;
        [SerializeField] private CollectableTypesEnum Jumper;

        [SerializeField] private Sprite Speaker;

        [SerializeField] private TileConfig SecondTileType;

        private WorldTilesService tilesService => ServiceLibrary.GetService<WorldTilesService>();
        private MessagesService messagesService => ServiceLibrary.GetService<MessagesService>();
        private CollectablesService collectablesService => ServiceLibrary.GetService<CollectablesService>();

        private Coroutine _behaviourCoroutine;

        public override void Play() {
            _behaviourCoroutine = CoroutineManager.Instance.StartRoutine(BehaviourRoutine());
        }

        public override void Stop() {
            if (_behaviourCoroutine != null) {
                CoroutineManager.Instance.StopRoutine(_behaviourCoroutine);
            }
            _behaviourCoroutine = null;
        }

        private IEnumerator BehaviourRoutine() {
            yield return new WaitForSecondsRealtime(4f);
            messagesService.ShowMessage("At last, runner...\n\nYou aren't tired, are you? Long way ahead.", Speaker);
            yield return WaitForMessageEnd();
            if (collectablesService.GetCollectedAmount(Common) == 0) {
                var movementTip = "Try to get those shards on the sides of the road.\n \"A\" and \"D\" may help";
                messagesService.ShowMessage(movementTip, Speaker);
                yield return new WaitForSeconds(10f);
            }
            if (collectablesService.GetCollectedAmount(Common) == 0) {
                var movementTip = "Chop-chop runner. Time is money.\nTry it. \"A\"";
                messagesService.ShowMessage(movementTip, Speaker);
                yield return new WaitForSeconds(10f);
            }
            if (collectablesService.GetCollectedAmount(Common) == 0) {
                messagesService.ShowMessage("Runner gotta run, huh?\n But life is more than just running forward.", Speaker);
                yield return new WaitForSeconds(10f);
            }
            while (collectablesService.GetCollectedAmount(Common) == 0) {
                messagesService.ShowMessage("Rrrrr...", Speaker);
                yield return new WaitForSeconds(10f);
            }

            var sprintTip = "You are promising. Let's get it over quick.\n\n Hold \"SHIFT\" to run faster but keep an eye on the erds";
            messagesService.ShowMessage(sprintTip, Speaker);
            tilesService.NormalizeAndLoadTile(SecondTileType);
            yield return null;

            _behaviourCoroutine = null;
        }

        private IEnumerator WaitForMessageEnd() {
            do {
                yield return new WaitForSeconds(4f);
                Debug.Log("message check");
            } while (messagesService.IsShowingMessage());
        }
    }
}