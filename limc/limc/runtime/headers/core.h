#ifndef LIM_CORE_H
#define LIM_CORE_H

#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <stdbool.h>

#include "tgc.h"
#include "utf8.h"

///////////////////
//// MEM ALLOC ////
///////////////////
static tgc_t gc;

void lim_gc_start(void* stk);
void lim_gc_stop();

void* lim_alloc(size_t size);
void* lim_alloc_leaf(size_t size);

/////////////
//// INT ////
/////////////
double lim_int2float(void* x);

//////////////
//// FOAT ////
//////////////
int lim_float2int(void* x);

/////////////
//// STR ////
/////////////

// immutable
struct lim_str_struct{
	size_t byte_len;
	size_t char_len;
	char data[];
};
typedef struct lim_str_struct* lim_str;

lim_str lim_literal2str(const char *literal);

void lim_puts(lim_str value);

#endif //LIM_CORE_H