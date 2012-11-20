#ifndef CORE_CONTEXT
#define CORE_CONTEXT

enum ColorChannel
{
	Red = 0,
	Green = 1,
	Blue = 2,
	GrayScale = 3
};

// Red = 0, Green = 1, Blue = 2, GrayScale = 3
long s_COLOR_CHANNEL;

/* Make sure functions are exported with C linkage under C++ compilers. */
#ifdef __cplusplus
extern "C"
{
#endif

HRESULT __cdecl InitTangraCore()
{
	s_COLOR_CHANNEL = 0;

	return S_OK;
};

HRESULT __cdecl Set8BitColourChannel(long colourChannel)
{
	s_COLOR_CHANNEL = colourChannel;

	return S_OK;
};

#ifdef __cplusplus
} // __cplusplus defined.
#endif

#endif