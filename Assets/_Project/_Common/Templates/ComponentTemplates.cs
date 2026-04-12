using Asteroids;
using Asteroids.BulletsFeature;
using Asteroids.Components;
using Asteroids.LocalInputFeature;
using Asteroids.StarshipInputControlFeature;
using Asteroids.StartshipsFeature;
using Asteroids.Views;
using DCFApixels;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.CameraController;
using Modules.Motion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ComponentTemplates
{
    [MetaID("Template_89922AB99501C6637E5655E8ABB1F04A")] class Template_89922AB99501C6637E5655E8ABB1F04A : ComponentTemplate<StarshipMovmentData> { }
    [MetaID("Template_50752EB99501945F2BDC43F6F3BFA134")] class Template_50752EB99501945F2BDC43F6F3BFA134 : ComponentTemplate<LocalInputReceiver> { }
    [MetaID("Template_E2272BB99501499999DA540D0B04344B")] class Template_E2272BB99501499999DA540D0B04344B : ComponentTemplate<VelocityDrag> { }
    [MetaID("Template_E9522BB9950115AD62A48B42DEA0B5ED")] class Template_E9522BB9950115AD62A48B42DEA0B5ED : ComponentTemplate<RigidTransform> { }
    [MetaID("Template_986A2BB9950107332FCFF43280030220")] class Template_986A2BB9950107332FCFF43280030220 : ComponentTemplate<Velocity> { }
    [MetaID("Template_2A352BCB9501428C5C1093F487E54C83")] class Template_2A352BCB9501428C5C1093F487E54C83 : ComponentTemplate<Asteroid> { }
    [MetaID("Template_AACF2FCB950110F7165389DD708F1671")] class Template_AACF2FCB950110F7165389DD708F1671 : ComponentTemplate<Starship> { }
    [MetaID("Template_D7729ECC9501F8D9D9868FBACC0F97B5")] class Template_D7729ECC9501F8D9D9868FBACC0F97B5 : ComponentTemplate<CameraSmoothFollowTarget> { }
    [MetaID("Template_CB79ECCA950122BC3E7CE6C8397659EB")] class Template_CB79ECCA950122BC3E7CE6C8397659EB : ComponentTemplate<Bullet> { }
    [MetaID("Template_395668D195011D0C128ED015E17731C5")] class Template_395668D195011D0C128ED015E17731C5 : ComponentTemplate<BoundsSphere> { }
    [MetaID("Template_60B433E69501E56AA53E7EED0A72BA83")] class Template_60B433E69501E56AA53E7EED0A72BA83 : ComponentTemplate<HitImmunity> { }
    
    [MetaID("Tempalte_B4F66F791607DD802EC7793F70E6D6D9")] class Tempalte_B4F66F791607DD802EC7793F70E6D6D9 : TagComponentTemplate<TouchToHitEmmiter> { }
    [MetaID("Template_35CA21CB95018BE256CAA78659C23B59")] class Template_35CA21CB95018BE256CAA78659C23B59 : ComponentTemplate<OutOfGameFieldBehavior> { }

    [MetaID("Template_9419F76F9D013E770643E72DB705C9F8")]
    class Template_9419F76F9D013E770643E72DB705C9F8 : ITemplateNode, IEcsComponentMember, ITypeMeta
    {
        [SerializeField]
        private ViewBase _viewPrefab;

        private readonly static TypeMeta _meta = typeof(ViewBase).GetMeta();
        public Type Type => _meta.Type;
        private readonly static string _name = $"{_meta.Name}(Auto Spawn)";
        public string Name => _name;
        public MetaColor Color => _meta.Color;
        public MetaDescription Description => _meta.Description;
        public MetaGroup Group => _meta.Group;
        public IReadOnlyList<string> Tags => _meta.Tags;
        public ITypeMeta BaseMeta => _meta;

        public void Apply(short worldID, int entityID)
        {
            var inst = _viewPrefab.Spawn(null, Vector3.zero, Quaternion.identity);
            EcsRefPool<ViewBase>.Apply(inst, entityID, worldID);
            inst.Connect((EcsWorld.GetWorld(worldID), entityID), false);
        }
    }
}
