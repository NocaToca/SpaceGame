using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StarColorData
{
    public static Color[] colors = new Color[]{
    new Color(0, 0, 0), //0
    new Color(0, 0, 0), //1
    new Color(0, 0, 0), //2
    new Color(0, 0, 0), //3
    new Color(0, 0, 0), //4
    new Color(0, 0, 0), //5
    new Color(0, 0, 0), //6
    new Color(0, 0, 0), //7
    new Color(0, 0, 0), //8
    new Color(0, 0, 0), //9
    new Color(1.0f, .0337f, 0), //10
    new Color(1.0f, .0592f, 0), //11
    new Color(1.0f, .0846f, 0), //12
    new Color(1.0f, .1096f, 0), //13
    new Color(1.0f, .1341f, 0), //14
    new Color(1.0f, .1578f, 0), //15
    new Color(1.0f, .1806f, 0), //16
    new Color(1.0f, .2025f, 0), //17
    new Color(1.0f, .2235f, 0), //18
    new Color(1.0f, .2434f, 0), //19
    new Color(1.0f, .2647f, .0033f), //20
    new Color(1.0f, .2889f, .0120f), //21
    new Color(1.0f, .3126f, .0219f), //22
    new Color(1.0f, .3360f, .0331f), //23
    new Color(1.0f, .3589f, .0454f), //24
    new Color(1.0f, .3814f, .0588f), //25
    new Color(1.0f, .3786f, .0734f), //26
    new Color(1.0f, .4250f, .0889f), //27
    new Color(1.0f, .4461f, .1054f), //28
    new Color(1.0f, .4668f, .1229f), //29
    new Color(1.0f, .4870f, .1411f), //30
    new Color(1.0f, .5067f, .1602f), //31
    new Color(1.0f, .5259f, .1800f), //32
    new Color(1.0f, .5447f, .2005f), //33
    new Color(1.0f, .5630f, .2216f), //34
    new Color(1.0f, .5809f, .2433f), //35
    new Color(1.0f, .5983f, .2655f), //36
    new Color(1.0f, .6153f, .2881f), //37
    new Color(1.0f, .6318f, .3112f), //38
    new Color(1.0f, .6480f, .3346f), //39
    new Color(1.0f, .6636f, .3583f), //40
    new Color(1.0f, .6789f, .3823f), //41
    new Color(1.0f, .6938f, .4066f), //42
    new Color(1.0f, .7038f, .4310f), //43
    new Color(1.0f, .7223f, .4556f), //44
    new Color(1.0f, .7360f, .4803f), //45
    new Color(1.0f, .7494f, .5051f), //46
    new Color(1.0f, .7623f, .5299f), //47
    new Color(1.0f, .7750f, .5548f), //48
    new Color(1.0f, .7872f, .5797f), //49
    new Color(1.0f, .7992f, .6045f), //50
    new Color(1.0f, .8108f, .6293f), //51
    new Color(1.0f, .8221f, .6541f), //52
    new Color(1.0f, .8330f, .6787f), //53
    new Color(1.0f, .8437f, .7032f), //54
    new Color(1.0f, .8541f, .7277f), //55
    new Color(1.0f, .8642f, .7519f), //56
    new Color(1.0f, .8740f, .7760f), //57
    new Color(1.0f, .8836f, .8000f), //58
    new Color(1.0f, .8929f, .8238f), //59
    new Color(1.0f, .9019f, .8473f), //60
    new Color(1.0f, .9107f, .8707f), //61
    new Color(1.0f, .9193f, .8939f), //62
    new Color(1.0f, .9276f, .9168f), //63
    new Color(1.0f, .9357f, .9396f), //64
    new Color(1.0f, .9436f, .9621f), //65
    new Color(1.0f, .9513f, .9844f), //66
    new Color(.9937f, .9526f, 1.0f), //67
    new Color(.9726f, .9395f, 1.0f), //68
    new Color(.9526f, .9270f, 1.0f), //69
    new Color(.9337f, .9150f, 1.0f), //70
    new Color(.9157f, .9035f, 1.0f), //71
    new Color(.8986f, .8925f, 1.0f), //72
    new Color(.8823f, .8819f, 1.0f), //73
    new Color(.8668f, .8718f, 1.0f), //74
    new Color(.8520f, .8621f, 1.0f), //75
    new Color(.8379f, .8527f, 1.0f), //76
    new Color(.8244f, .8437f, 1.0f), //77
    new Color(.8115f, .8351f, 1.0f), //78
    new Color(.7992f, .8268f, 1.0f), //79
    new Color(.7874f, .8187f, 1.0f), //80
    new Color(.7761f, .8110f, 1.0f), //81
    new Color(.7652f, .8035f, 1.0f), //82
    new Color(.7548f, .7963f, 1.0f), //83
    new Color(.7449f, .7894f, 1.0f), //84
    new Color(.7353f, .7827f, 1.0f), //85
    new Color(.7260f, .7762f, 1.0f), //86
    new Color(.7172f, .7699f, 1.0f), //87
    new Color(.7086f, .7638f, 1.0f), //88
    new Color(.7004f, .7579f, 1.0f), //89
    new Color(.6925f, .7522f, 1.0f), //90
    new Color(.6848f, .7467f, 1.0f), //91
    new Color(.6774f, .7414f, 1.0f), //92
    new Color(.6703f, .7362f, 1.0f), //93
    new Color(.6635f, .7311f, 1.0f), //94
    new Color(.6568f, .7263f, 1.0f), //95
    new Color(.6504f, .7215f, 1.0f), //96
    new Color(.6442f, .7169f, 1.0f), //97
    new Color(.6382f, .7124f, 1.0f), //98
    new Color(.6324f, .7081f, 1.0f), //99
    new Color(.6268f, .7039f, 1.0f), //100
    new Color(.6213f, .6998f, 1.0f), //101
    new Color(.6162f, .6958f, 1.0f), //102
    new Color(.6109f, .6919f, 1.0f), //103
    new Color(.6060f, .6881f, 1.0f), //104
    new Color(.6012f, .6844f, 1.0f), //105
    new Color(.5965f, .6808f, 1.0f), //106
    new Color(.5919f, .6773f, 1.0f), //107
    new Color(.5875f, .6739f, 1.0f), //108
    new Color(.5833f, .6706f, 1.0f), //109
    new Color(.5791f, .6674f, 1.0f), //110
    new Color(.5750f, .6642f, 1.0f), //111
    new Color(.5711f, .6611f, 1.0f), //112
    new Color(.5673f, .6581f, 1.0f), //113
    new Color(.5636f, .6552f, 1.0f), //114
    new Color(.5599f, .6523f, 1.0f), //115
    new Color(.5564f, .6495f, 1.0f), //116
    new Color(.5530f, .6468f, 1.0f), //117
    new Color(.5496f, .6441f, 1.0f), //118
    new Color(.5463f, .6415f, 1.0f), //119
    new Color(.5187f, .6389f, 1.0f) //120


    







    };
}
