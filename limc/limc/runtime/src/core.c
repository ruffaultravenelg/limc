#include "core.h"
#include <string.h>

///////////////////
//// MEM ALLOC ////
///////////////////
void lim_gc_start(void* stk) {
	tgc_start(&gc, stk);
}
void lim_gc_stop() {
	tgc_stop(&gc);
}

void* lim_alloc(size_t size) {
	void* ptr = tgc_alloc(&gc, size);
	if (ptr == NULL) {
		// todo: PANIC
		exit(EXIT_FAILURE);
	}
	return ptr;
}

void* lim_alloc_leaf(size_t size) {
	void* ptr = tgc_alloc_opt(&gc, size, TGC_LEAF, NULL);
	if (ptr == NULL) {
		// todo: PANIC
		exit(EXIT_FAILURE);
	}
	return ptr;
}

/////////////
//// STR ////
/////////////
lim_str lim_literal2str(const char *literal) {
    size_t bytes = strlen(literal);
    size_t chars = utf8len(literal);

    lim_str s = (lim_str)lim_alloc_leaf(sizeof(struct lim_str_struct) + bytes + 1);

    s->byte_len = bytes;
    s->char_len = chars;
    
    memcpy(s->data, literal, bytes);
    s->data[bytes] = '\0';

    return s;
}

void lim_puts(lim_str value) {
	if (!value) return;
    fwrite(value->data, sizeof(char), value->byte_len, stdout);
	putchar('\n');
}