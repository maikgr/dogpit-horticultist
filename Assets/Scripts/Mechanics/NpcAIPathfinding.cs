namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using DG.Tweening;
    using Pathfinding;
    using Horticultist.Scripts.Core;

    [RequireComponent(typeof(Seeker))]
    public class NpcAIPathfinding : MonoBehaviour
    {
        [SerializeField] private Animator bodyAnimator;
        [SerializeField] private Transform visualParent;
        private Seeker seeker;

        private void Awake() {
            seeker = GetComponent<Seeker>();
        }

        private void Start() {
            StartCoroutine(TryWalking());
        }

        private void OnEnable() {
            seeker.pathCallback += OnPathComplete;
        }

        private void OnDisable() {
            seeker.pathCallback -= OnPathComplete;
        }

        private IEnumerator TryWalking()
        {
            bodyAnimator.SetBool("isWalking", false);
            yield return new WaitForSeconds(Random.Range(0f, 5f));
            bodyAnimator.SetBool("isWalking", true);
            var currentNode = AstarPath.active.GetNearest(transform.position).node;
            var target = (Vector3)GetDestination(currentNode).position;
            
            seeker.StartPath(transform.position, target);
        }
        
        private GraphNode GetDestination(GraphNode currentNode)
        {
            GraphNode targetNode;
            var maxTries = 20;
            do
            {
                var grid = AstarPath.active.data.gridGraph;

                targetNode = grid.nodes[Random.Range(0, grid.nodes.Length)];
                maxTries -= 1;
            }
            while (!PathUtilities.IsPathPossible(currentNode, targetNode) || maxTries < 0);

            return targetNode;
        }

        private void OnPathComplete (Path p) {
            if (p.error) {
                StopCoroutine(TryWalking());
                StartCoroutine(TryWalking());
                return;
            }
            var sequence = DOTween.Sequence();
            for (var i = 1; i < p.vectorPath.Count; ++i)
            {
                sequence.Append(
                    transform.DOMove((Vector3)p.vectorPath[i], 2f)
                        .SetEase(Ease.Linear)
                );
                if (p.vectorPath[i].x < transform.position.x)
                {
                    sequence.Join(
                        visualParent.DORotate(new Vector3(0, 180, 0), 0.5f)
                    );
                }
                else
                {
                    sequence.Join(
                        visualParent.DORotate(new Vector3(0, 0, 0), 0.5f)
                    );
                }
            }
            sequence.OnComplete(() => {
                StopCoroutine(TryWalking());
                StartCoroutine(TryWalking());
            });
        }

    }

}