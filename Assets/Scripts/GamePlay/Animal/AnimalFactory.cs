using UnityEngine;

namespace PickAndMatch.Gameplay.Animal
{
    public class AnimalFactory
    {
        private readonly Transform parent;

        public AnimalFactory(Transform parent)
        {
            this.parent = parent;
        }

        // prefab: 1 prefab Animal DUY NHẤT dùng chung cho mọi loại con vật.
        // AnimalData chỉ còn cung cấp Type + Sprite, không dùng field Prefab của nó nữa.
        public Animal Create(
            Animal prefab,
            AnimalData data,
            Vector3 position,
            Sprite backFrameSprite,
            Sprite frontFrameSprite)
        {
            if (data == null)
            {
                Debug.LogError("AnimalData is null.");
                return null;
            }

            if (prefab == null)
            {
                Debug.LogError(
                    "Animal prefab chưa được gán (BoardManager > Animal Prefab).");
                return null;
            }

            Animal animal = Object.Instantiate(
                prefab,
                position,
                Quaternion.identity,
                parent);

            animal.Initialize(
                data,
                backFrameSprite,
                frontFrameSprite);

            return animal;
        }
    }
}