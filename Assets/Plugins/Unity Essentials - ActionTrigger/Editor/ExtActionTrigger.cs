using UnityEngine;
using UnityEditor;
using UnityEssentials.ActionTrigger.Trigger;
using UnityEssentials.Geometry.MovableShape;
using UnityEssentials.ActionTrigger.Actions.example;
using UnityEssentials.CrossSceneReference;
using UnityEssentials.SceneWorkflow;
using UnityEssentials.ActionTrigger.entity;

namespace UnityEssentials.ActionTrigger.editor
{
	public static class ExtActionTrigger
    {
        #region Setup System
        [MenuItem("GameObject/Unity Essentials/ActionTrigger/Popupate System and Exemple", false, 11)]
        private static void PopulateSystem()
        {
            if (GameObject.FindObjectOfType<DependencyInjectorSingleton>())
            {
                Debug.LogError("There is already a DependencyInjectorSingleton in the scene!");
                return;
            }

            GameObject systemParent = new GameObject("[Action Trigger] System");
            GameObject dependencyInjectorSingleton = new GameObject("DependencyInjectorSingleton    (must be only one)", typeof(DependencyInjectorSingleton));
            dependencyInjectorSingleton.transform.SetParent(systemParent.transform);
            GameObject entityLister = new GameObject("Entity Lister      (keep track of every EntityBase in every Scenes)", typeof(EntityListerInScenes));
            entityLister.transform.SetParent(systemParent.transform);
            Undo.RegisterCreatedObjectUndo(systemParent, systemParent.name);

            Selection.activeGameObject = null;
            TriggerZoneNoPhysics zoneNoPhysics = CreateTrigger();
            zoneNoPhysics.gameObject.name = "[Action Trigger] Zone With Custom OnEnter/Exit Calculation";

            Selection.activeGameObject = null;
            TriggerZonePhysics zonePhysics = CreateTriggerPhysic();
            zonePhysics.gameObject.name = "[Action Trigger] Zone using OnEnter/Exit of the physic engine";
            zonePhysics.transform.position += new Vector3(3, 0, 0);

            Selection.activeGameObject = null;
            Entity entity = CreateEntity();
            entity.gameObject.name = "[Action Trigger] Entity";
            entity.transform.position += new Vector3(0, 0, 3);

            Selection.activeGameObject = null;
            Entity entityPhysic = CreateEntityPhysic();
            entityPhysic.gameObject.name = "[Action Trigger] Entity with RigidBody";
            entityPhysic.transform.position += new Vector3(3, 0, 3);
        }

        #endregion

        #region Create Entity
        [MenuItem("GameObject/Unity Essentials/ActionTrigger/Entity", false, 11)]
        private static Entity CreateEntity()
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Entity";
            if (Selection.activeGameObject != null)
            {
                sphere.transform.SetParent(Selection.activeGameObject.transform);
            }


            Entity entity = sphere.AddComponent<Entity>();
            ExtGameObjectIcon.SetIcon(sphere, ExtGameObjectIcon.LabelIcon.Red);

            Undo.RegisterCreatedObjectUndo(sphere, sphere.name);
            Selection.activeGameObject = sphere;
            SceneView.lastActiveSceneView.MoveToView(sphere.transform);
            GameObjectUtility.EnsureUniqueNameForSibling(sphere);

            return (entity);
        }

        [MenuItem("GameObject/Unity Essentials/ActionTrigger/Entity Physic", false, 11)]
        private static Entity CreateEntityPhysic()
        {
            Entity entity = CreateEntity();
            Rigidbody rigidbody = entity.gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            return (entity);
        }
        #endregion

        #region Create Trigger & Action

        [MenuItem("GameObject/Unity Essentials/ActionTrigger/Trigger No Physic", false, 11)]
        private static TriggerZoneNoPhysics CreateTrigger()
        {
            GameObject triggerGameObject = CreateGameObject("Trigger No Physic", typeof(TriggerZoneNoPhysics));
            TriggerZoneNoPhysics trigger = triggerGameObject.GetComponent<TriggerZoneNoPhysics>();
            ExtGameObjectIcon.SetIcon(triggerGameObject, ExtGameObjectIcon.Icon.CircleYellow);

            SerializedObject triggerObject = new SerializedObject(trigger);
            MovableCube cube = AddZoneShape<MovableCube>(trigger, triggerObject);
            ExampleAction action = AddActionGuid(trigger);
            LinkActionToTriggerZoneArray(trigger, triggerObject, action);

            return (trigger);
        }

        
        [MenuItem("GameObject/Unity Essentials/ActionTrigger/Trigger Physic", false, 11)]
        private static TriggerZonePhysics CreateTriggerPhysic()
        {
            GameObject triggerGameObject = CreateGameObject("Trigger Physic", typeof(TriggerZonePhysics));
            TriggerZonePhysics trigger = triggerGameObject.GetComponent<TriggerZonePhysics>();
            ExtGameObjectIcon.SetIcon(triggerGameObject, ExtGameObjectIcon.Icon.CircleYellow);

            SerializedObject triggerObject = new SerializedObject(trigger);
            BoxCollider collider = trigger.gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            ExampleAction action = AddActionGuid(trigger);
            LinkActionToTriggerZoneArray(trigger, triggerObject, action);

            return (trigger);
        }

        /// <summary>
        /// Generate the main gameObject with a TriggerZone components,
        /// place it correctly
        /// </summary>
        /// <typeparam name="T">type ofTriggerZone</typeparam>
        /// <param name="nameType">name of gameObject</param>
        /// <returns>TriggerZone reference created</returns>
        private static GameObject CreateGameObject(string nameType = "New GameObject", params System.Type[] components)
        {
            GameObject zone = new GameObject(nameType, components);

            if (Selection.activeGameObject != null)
            {
                zone.transform.SetParent(Selection.activeGameObject.transform);
            }

            Undo.RegisterCreatedObjectUndo(zone, nameType);
            Selection.activeGameObject = zone;
            SceneView.lastActiveSceneView.MoveToView(zone.transform);
            GameObjectUtility.EnsureUniqueNameForSibling(zone);
            return (zone);
        }

        /// <summary>
        /// Add a movable shape to the TriggerZone
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        private static T AddZoneShape<T>(TriggerZoneNoPhysics trigger, SerializedObject triggerObject)
            where T : MovableShape
        {
            T shape = trigger.gameObject.AddComponent(typeof(T)) as T;
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<T>(triggerObject.GetPropertie("_shape"), trigger.transform);
            return shape;
        }

        private static ExampleAction AddActionGuid(TriggerZone trigger)
        {
            GameObject actionChild = new GameObject("Action of " + trigger.name);
            actionChild.transform.SetParent(trigger.transform);
            actionChild.transform.localPosition = new Vector3(0, 0, 1);
            ExtGameObjectIcon.SetIcon(actionChild, ExtGameObjectIcon.Icon.CircleRed);
            ExampleAction action = actionChild.AddComponent(typeof(ExampleAction)) as ExampleAction;
            return action;
        }

        private static void LinkActionToTriggerZoneArray(TriggerZone trigger, SerializedObject triggerObject, ExampleAction action)
        {
            SerializedProperty actionProperty = triggerObject.FindProperty("_actions");
            actionProperty.arraySize++;
            triggerObject.ApplyModifiedProperties();
            GuidReference guid = new GuidReference(action);
            trigger.SetActionByIndex(guid, 0);
        }
        #endregion
    }
}