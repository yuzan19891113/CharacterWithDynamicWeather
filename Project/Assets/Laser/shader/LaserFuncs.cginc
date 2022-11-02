#ifndef LASERFUNCS_INCLUDED
#define LASERFUNCS_INCLUDED

#include "UnityPBSLighting.cginc"

float toZerobtOne(inout float x)
{
    int y = floor(x);
    x -= y;
    return x;
}

float3 HUEtoRGB(float H)
{
    toZerobtOne(H);
	float R = abs(6 * H - 3) - 1;
	float G = 2 - abs(6 * H - 2);
	float B = 2 - abs(6 * H - 4);
	return saturate(float3(R, G, B));
}

float3 HSVtoRGB(float3 HSV)
{
	float3 RGB = HUEtoRGB(HSV.x);
	return ((RGB - 1) * HSV.y + 1) * HSV.z;
}

float3 RGBtoHSV(float3 RGB)
{
    float Cmax = max(max(RGB.r, RGB.g), RGB.b);
    float Cmin = min(min(RGB.r, RGB.g), RGB.b);
    float delta = Cmax - Cmin;

    float H = 0;
    if (delta == 0)
        H = 0;
    else if (Cmax == RGB.r && RGB.g > RGB.b)
        H = (RGB.g - RGB.b) / (6 * delta);
    else if (Cmax == RGB.r && RGB.b > RGB.g)
        H = 1 + (RGB.g - RGB.b) / (6 * delta);
    else if (Cmax == RGB.g)
        H = (RGB.b - RGB.r) / (6 * delta) + 1.0/3.0;
    else if (Cmax == RGB.b)
        H = (RGB.r - RGB.g) / (6 * delta) + 2.0/3.0;

    float S = Cmax == 0 ? 0 : delta / Cmax;

    return float3(H, S, Cmax);
}

float GetCostheta(float3 n, float3 t, float3 v, float dir, sampler2D dirmap, float2 uv)
{
#ifdef _ANISOCOLOR
    float3 b = normalize(cross(t, n));

    #ifdef _COLORDIRMAP
    float colordir_angle = tex2D(dirmap, uv).r * 3.1415926535;  // pi
    float costheta_rate = tex2D(dirmap, uv).g;
    #else
    float colordir_angle = dir * 0.0174532925;  // pi/180
    float costheta_rate = 1;
    #endif

    float3 colordir_intangentspace = float3(cos(colordir_angle), sin(colordir_angle), 0);
    float3 colordir = normalize(t * colordir_intangentspace.x + b * colordir_intangentspace.y + n * colordir_intangentspace.z);

    float3 dir_perpendicular = normalize(cross(colordir, n));
    float3 temp = cross(dir_perpendicular, v);
    float3 colordir_v = normalize(cross(temp, dir_perpendicular));

    return 1 - costheta_rate * (1 - abs(dot(n, colordir_v)));
#else
    return dot(n, v);
#endif
}

float GetX(float costheta)
{
#ifdef _SINCONTROL
	return sqrt(1 - costheta * costheta);
#else
    return 1 - costheta;
#endif
}

#ifdef _MANUALCONTROL

//��һ�������������䣬��������������Ĥ��͸���ģ�����������Щ�̼������Ǹ����������߲ʣ���͸���ģ�����������
float3 ThetatoRGB(float3 normaldir, float3 tangentdir, float3 viewdir, float saturation, float brightness, float lightspan, float lightoffset, sampler2D offsetmap, int maptype, float colordir, sampler2D colordirmap, float2 uv = float2(0, 0))
{
	float costheta = GetCostheta(normaldir, tangentdir, viewdir, colordir, colordirmap, uv);
    float H = GetX(costheta) * lightspan + lightoffset;
    if (maptype == 1)
    {
        H += tex2D(offsetmap, uv).r;
    }
    else if (maptype == 2)
    {
        H += tex2D(offsetmap, uv).g;
    }

    return HSVtoRGB(float3(H, saturation, brightness));
}

#else

float3 ThetatoRGB(float3 normaldir, float3 tangentdir, float3 viewdir, float saturation, float brightness, sampler2D huemap, sampler2D offsetmap, int maptype, float colordir, sampler2D colordirmap, float2 uv = float2(0, 0))
{
	float costheta = GetCostheta(normaldir, tangentdir, viewdir, colordir, colordirmap, uv);
    float x = GetX(costheta);
    if (maptype == 1)
    {
        x += tex2D(offsetmap, uv).r;
    }
    else if (maptype == 2)
    {
        x += tex2D(offsetmap, uv).g;
    }

    x = 0.001 + 0.998 * saturate(x);
    float3 HSV = RGBtoHSV(tex2D(huemap, float2(x, 0.5)).rgb);

    return HSVtoRGB(float3(HSV.x, HSV.y * saturation, HSV.z * brightness));
}

#endif

/************************************************************************/











//��һ������ʵ�����ַ��������䣬���򿴹�ȥ����ǿ�Ⱥ�����������֮��ķ��򿴹�ȥ����ǿ�ȱȽ�ǿ
float3 ThetatoRGB2(float3 NormalDir, float3 ViewDir, float saturation, float brightness, float lightspan, float lightoffset, sampler2D map, int maptype = 0, float2 uv = float2(0, 0))
{
	float theta = dot(NormalDir, ViewDir);
	float H = (1 - theta) * lightspan + lightoffset;
    float sat_rate = 1 - 0.5 * pow(abs(theta), 6);

    if (maptype == 1)
    {
        H += tex2D(map, uv).r;
    }
    else if (maptype == 2)
    {
        H += tex2D(map, uv).g;
    }

    return HSVtoRGB(float3(H, saturation * sat_rate, brightness));
}

float cuthue(float H)
{
    while (H > 2)
        H -= 2;
    while (H < 0)
        H += 2;
    if (H > 1)
        H = 1;
    return H;
}

float3 ThetatoRGB3(float3 NormalDir, float3 TangentDir, float3 ViewDir, UnityLight mainlight, float saturation, float brightness, float lightspan, float lightoffset, sampler2D map, int maptype = 0, float2 uv = float2(0, 0))
{
    const float MY_PI = 3.1415926536;
    const float MY_PI_devidetwo = 1.5707963268;
    const float MY_PI_devidefour = 0.7853981634;
    const float MY_PI_devidesixteen = 0.1963495408;
    const float INV_PI_mulsixteen = 5.092958180;

    float theta = dot(NormalDir, ViewDir);
	float H = -(1 - theta) * lightspan + lightoffset;
    
    float3 LightDir = mainlight.dir;
    float3 temp = normalize(cross(LightDir, NormalDir));
    float3 TangentPlainLightDir = normalize(cross(NormalDir, temp));
    float phi = dot(TangentDir, TangentPlainLightDir);

    float sat_rate2 = 0;
    float angle = acos(phi);
    
    if (angle < -MY_PI)
        angle += MY_PI;
    if (angle > MY_PI)
        angle -= MY_PI;

    if (angle < 0 && angle > -MY_PI_devidefour)
        angle += MY_PI_devidefour;
    else if (angle > MY_PI_devidefour)
        angle -=  MY_PI_devidefour;
    else if (angle < -MY_PI_devidefour)
        angle +=  MY_PI_devidetwo;

    if (angle < MY_PI_devidesixteen && angle > 0)
        sat_rate2 = 1 - angle * INV_PI_mulsixteen;
    else if (angle > 3 * MY_PI_devidesixteen && angle < MY_PI_devidefour)
        sat_rate2 = angle * INV_PI_mulsixteen - 3;

    H = cuthue(H);

    float sat_rate = 1;
    if (H < 1.0/12)
        sat_rate = 12 * H;
    else if (H > 9.0/12 && H < 10.0/12)
        sat_rate = 10 - 12 * H;
    else if (H > 10.0/12)
        sat_rate = 0;
    else
        sat_rate = 1;
    
    float3 cc = HSVtoRGB(float3(H, saturation * sat_rate * sat_rate2, brightness));

    return cc;
}

float CalcSinAngle(float3 t, float3 b, float3 n, float3 v)
{
    float3 temp = cross(b, v);
    float3 target = normalize(cross(temp, b));
    float cosA = dot(n, target);
    float s = dot(t, target);
    return sign(s) * sqrt(1 - cosA*cosA);
}

float3 ThetatoRGB4(float3 NormalDir, float3 TangentDir, float3 ViewDir, UnityLight mainlight, float saturation, float brightness, float lightspan, float lightoffset, sampler2D map, int maptype = 0, float2 uv = float2(0, 0))
{
    float3 b2 = normalize(cross(TangentDir, NormalDir));
    float3 t2 = normalize(cross(NormalDir, b2));
    float3 n2 = NormalDir;

    float theta = 6.283185307 * uv.y;
    float sinR = sin(theta);
    float cosR = cos(theta);
    float FcosR = 1 - cosR;
    float3 R1 = float3( n2.x*n2.x*FcosR+cosR , n2.x*n2.y*FcosR-n2.z*sinR , n2.x*n2.z*FcosR+n2.y*sinR );
    float3 R2 = float3( n2.x*n2.y*FcosR+n2.z*sinR , n2.y*n2.y*FcosR+cosR , n2.y*n2.z*FcosR-n2.x*sinR );
    float3 R3 = float3( n2.x*n2.z*FcosR-n2.y*sinR , n2.y*n2.z*FcosR+n2.x*sinR , n2.z*n2.z*FcosR+cosR );

    float3 b2_rotated = float3(dot(b2, R1), dot(b2, R2), dot(b2, R3));
    float3 t2_rotated = float3(dot(t2, R1), dot(t2, R2), dot(t2, R3));

    float3 LightDir = mainlight.dir;
    float deltasin = abs(CalcSinAngle(t2_rotated, b2_rotated, n2, ViewDir) + CalcSinAngle(t2_rotated, b2_rotated, n2, LightDir));
    float H = deltasin * lightspan + lightoffset;
    return HSVtoRGB(float3(H, saturation, brightness));
}

#endif //LASERFUNCS_INCLUDED