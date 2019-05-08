﻿using System;

namespace NetsphereScnTool.Scene
{
    [Flags]
    public enum RenderState
    {
        None = 0,
        NoLight = 1,
        AlphaBlend1 = 2,
        AlphaTest = 4,
        NoCull = 8,
        MoveWithMouse = 16,
        AlphaBlend2 = 32,
        NoDepthWrite = 64,
        Shader = 128,
        NoFog = 512,
        NoMipmap = 2048,
        Shadow = 8192,
        Water = 32768,
        MotBlur = 65536,
        Dark = 131072
    }

    public enum ChunkType : uint
    {
        Box = 0x25ADF0D1, // fumbi, spawns, deadzones
        ModelData = 0x081098F8, // Game::CActorGeometry
        Bone = 0x6D411AD1, // CoreLib::Scene::CBone
        SkyDirect1 = 0xC3E8BE62,
        BoneSystem = 0x5E74333F,
        Shape = 0xADEE38A2
    }

    public enum ParentGrade : uint
    {
        Father,
        Child,
        Grandson
    }
}
