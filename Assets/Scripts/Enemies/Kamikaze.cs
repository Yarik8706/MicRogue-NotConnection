using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using MainScripts;



namespace Enemies
{
    public class Kamikaze : LifeFire
    {
        private readonly System.Random _random = new();
        private int _stepsToExplosion;
        private int _maxSteps;
        private int _stepsCount;
        public GameObject fireDust;
        private float _distancePosition;

        public override void Awake()
        {
            base.Awake();
            _maxSteps = _random.Next(4, 12);
            _stepsToExplosion = _random.Next(3, _maxSteps);
        }

        public override void Active()
        {
            _stepsCount++;
            isTurnOver = false;
            _distancePosition = Vector2.Distance(GameManager.player.transform.position, transform.position);

            if (GameManager.player == null)
            {
                TurnOver();
                return;
            }

            if ((_stepsCount != _stepsToExplosion) && (_distancePosition > 1.5f))
            {
                SelectAction(
                    SelectMovePosition(
                        MoveCalculation(VariantsPositionsNow(variantsPositions))), 
                    GameManager.player.transform.position);
            }
            else
            {
                Died();
            }

        }

        public override void Died()
        {
            var dustPositions = PositionCalculation(transform.position, VariantsPositionsNow(firePosition), noFireLayer, boxCollider2D);

            foreach (var coordinate in dustPositions)
            {
                Instantiate(fireDust, coordinate, Quaternion.identity);
            }

            base.Died();
        }
        
        private Vector2 SelectionOfTheNearestPosition(
            Vector2[] positions, Vector2 playerPosition, System.Random random)
        {
            var distance = new List<float>();
            var currentPositions = positions.ToList();
            int chanceToRightWay = random.Next(100);
            int indexFirstPosition;
            Vector2 firstPosition;

            foreach (var coordinate in positions)
            {
                distance.Add(Vector2.Distance(coordinate, playerPosition));
            }

            switch (distance.Count)
            {
                case 1:
                    return currentPositions[distance.IndexOf(distance.Min())];
                case >= 2:
                    indexFirstPosition = distance.IndexOf(distance.Min());
                    firstPosition = currentPositions[distance.IndexOf(distance.Min())];

                    distance.RemoveAt(indexFirstPosition);
                    currentPositions.RemoveAt(indexFirstPosition);

                    return chanceToRightWay < 65 ? firstPosition : currentPositions[distance.IndexOf(distance.Min())];
            }

            return transform.position;
        }
        
        private Vector2 SelectMovePosition(Vector2[] theVariantsPositions)
        {
            return SelectionOfTheNearestPosition(
                MoveCalculation(VariantsPositionsNow(variantsPositions)), 
                GameManager.player.transform.position, 
                _random);
        }

        public Vector2[] ExplosiveCoordinates(Vector2[] coordinates)
        {
            var explosiveCoordinates = new List<Vector2>();
            var position = transform.position;

            for (int i = 0; i < coordinates.Length; i++)
            {
                explosiveCoordinates.Add(new Vector2(position.x + coordinates[i].x, position.y + coordinates[i].y));
            }

            return explosiveCoordinates.ToArray();
        }
    }
}