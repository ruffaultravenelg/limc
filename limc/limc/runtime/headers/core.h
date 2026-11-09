#ifndef LIM_CORE_H
#define LIM_CORE_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <stdbool.h>

/////////////
//// INT ////
/////////////
#define lim_INT2FLOAT(x) ((float)x)

//////////////
//// FOAT ////
//////////////
#define lim_FLOAT2INT(x) ((int)round(x))

/////////////////
//// LIM STR ////
/////////////////

typedef struct {
	char* c_str;
	size_t len;
} lim_str;

void lim_core_puts(char* str);

#endif //LIM_CORE_H