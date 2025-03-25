using Asteroids.Components;
using Asteroids.ControlsFeature;
using Asteroids.MovementFeature;
using Asteroids.StarshipMovmentFeature;
using DCFApixels.DragonECS;

namespace ComponentTemplates
{
    [MetaID("Template_89922AB99501C6637E5655E8ABB1F04A")] class Template_89922AB99501C6637E5655E8ABB1F04A : ComponentTemplate<StarshipMovmentData> { }
    [MetaID("Template_50752EB99501945F2BDC43F6F3BFA134")] class Template_50752EB99501945F2BDC43F6F3BFA134 : ComponentTemplate<AxisControlData> { }
    [MetaID("Template_E2272BB99501499999DA540D0B04344B")] class Template_E2272BB99501499999DA540D0B04344B : ComponentTemplate<RigidbodyData> { }
    [MetaID("Template_E9522BB9950115AD62A48B42DEA0B5ED")] class Template_E9522BB9950115AD62A48B42DEA0B5ED : ComponentTemplate<TransformData> { }
    [MetaID("Template_986A2BB9950107332FCFF43280030220")] class Template_986A2BB9950107332FCFF43280030220 : ComponentTemplate<Velocity> { }
    [MetaID("Template_35CA21CB95018BE256CAA78659C23B59")] class Template_35CA21CB95018BE256CAA78659C23B59 : ComponentTemplate<WrapAroundScreenMarker> { }
    [MetaID("Template_2A352BCB9501428C5C1093F487E54C83")] class Template_2A352BCB9501428C5C1093F487E54C83 : ComponentTemplate<Asteroid> { }
    [MetaID("Template_AACF2FCB950110F7165389DD708F1671")] class Template_AACF2FCB950110F7165389DD708F1671 : ComponentTemplate<Starship> { }

    [MetaID("Template_CB79ECCA950122BC3E7CE6C8397659EB")] class Template_CB79ECCA950122BC3E7CE6C8397659EB : TagComponentTemplate<Bullet> { }
    [MetaID("Template_4A1123CB9501B7933D75AA7248F28132")] class Template_4A1123CB9501B7933D75AA7248F28132 : TagComponentTemplate<KillOutsideMarker> { }
}
