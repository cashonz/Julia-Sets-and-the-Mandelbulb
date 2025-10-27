//Distance estimator of Julia sets from Hart et. al(1989) equation (6). 
float DEJulia(float3 p, int maxIterations, float3 rotationAxis, float theta, float4 c, float escapeRadius)
{
    float4 z = float4(0, p);
    float4 zp = float4(0, 0, 0, 1); //derivative

    for(int i = 0; i < maxIterations; i++)
    {
        zp = 2 * quatMult(z, zp) + 1;
        zp = RotateQuaternion(zp, rotationAxis, theta);
        z = quatSq(z) + c;
        z = RotateQuaternion(z, rotationAxis, theta);

        if(dot(z, z) > escapeRadius) break;
    }

    return 0.5 * length(z) * log(length(z)) / (length(zp));
}
//Distance estimator of mandelbulb from http://blog.hvidtfeldts.net/index.php/2011/09/distance-estimated-3d-fractals-v-the-mandelbulb-different-de-approximations/
float DEMandelbulb(float3 pos, int maxIterations, float escapeRadius, float powVal) 
{
	float3 z = pos;
	float dr = 1.0;
	float r = 0.0;
	for (int i = 0; i < maxIterations ; i++) {
		r = length(z);
		if (r>escapeRadius) break;
		
		// convert to polar coordinates
		float theta = acos(z.z/r);
		float phi = atan2(z.y,z.x);
		dr =  pow(r, powVal-1.0)*powVal*dr + 1.0;
		
		// scale and rotate the point
		float zr = pow(r,powVal);
		theta = theta*powVal;
		phi = phi*powVal;
		
		// convert back to cartesian coordinates
		z = zr*float3(sin(theta)*cos(phi), sin(phi)*sin(theta), cos(theta));
		z+=pos;
	}
	return 0.5*log(r)*r/dr;
}

