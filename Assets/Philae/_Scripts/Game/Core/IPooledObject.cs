﻿using UnityEngine;

namespace Philae.Core
{
    public interface IPooledObject
    {
        /// <summary>
        /// appelé lors du spawn de l'objet depuis la pool !
        /// </summary>
        void OnObjectSpawn();
        void OnDesactivePool();
    }
}