#ifndef LIM_CORE_H
#define LIM_CORE_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <stdbool.h>

// LIM str
typedef struct {
	char* c_str;
	size_t len;
} lim_str;

void lim_core_puts(char* str);

#endif //LIM_CORE_H