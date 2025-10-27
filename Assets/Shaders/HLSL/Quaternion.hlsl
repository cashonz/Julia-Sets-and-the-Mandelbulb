static const float PI = 3.14159265f;

/*
###########################################################################################
quatMult and quatSq from https://www.cs.cmu.edu/~kmcrane/Projects/QuaternionJulia/paper.pdf
###########################################################################################
*/
//Quaternion multiplication
float4 quatMult( float4 q1, float4 q2 )
{
    float4 r;
    r.w = q1.w*q2.w - dot( q1.xzy, q2.xzy );
    r.xzy = q1.w*q2.xzy + q2.w*q1.xzy + cross( q1.xzy, q2.xzy );
    return r;
}
//Fast square for quaternions
float4 quatSq( float4 q )
{
    float4 r;
    r.w = q.w*q.w - dot( q.xzy, q.xzy );
    r.xzy = 2*q.w*q.xzy;
    return r;
}
//Quaternion power function
float4 quatPow(float4 q, int exp)
{
    if(exp == 0) return float4(0, 0, 0, 1);
    if(exp == 1) return q;

    float4 curr = q;
    for(int i = 0; i < exp - 1; i++)
    {
        curr = quatMult(curr, q);
    }

    return curr;
}
//Get Angle between axes;
float4 AngleAxis(float angle, float3 axis)
{
    axis = normalize(axis);
    axis *= sin(angle);
    return float4(axis.x, axis.y, axis.z, cos(angle));
}
//Rotates and axis such that one of it's axes follow the target direction
float4 FromToRotation(float3 from, float3 to)
{
    float3 axis = cross(from, to);
    float angle = acos(dot(from, to)); //HLSL acos returns radians, no need to convert from degrees
    return AngleAxis(angle, normalize(axis));
}
//Rotates a quaternion about an axis 
float4 RotateQuaternion(float4 z, float3 axis, float theta)
{
    axis = normalize(axis);
    float halfTheta = radians(theta) * 0.5;
    float s = sin(halfTheta);
    float c = cos(halfTheta);
    float4 r = float4(axis * s, c);
    float4 rConj = float4(-r.xyz, c);

    return quatMult(quatMult(r, z), rConj);
}