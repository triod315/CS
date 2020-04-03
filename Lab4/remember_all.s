.include "defs.h"
.section .bss
argc: .quad 0
argv: .quad 0
ptr: .quad 0
fd: .quad 0
len: .quad 0

.section .text
.global _start

_start:
        movq (%rsp), %rbx             /* rbx = *rsp Кількість аргументів*/
        movq %rbx, argc               /* argc = %rsp */
        movq 16(%rsp), %rcx           /* rcx = rsp + 16 */
        movq %rcx, argv               /* argv = %rcx */

        cmpq $2, argc   /*if argc !=2 goto exit*/
        jne exit

        /*open file*/
        movq $2, %rax /*номер sys_open у eax регістр*/
        movq argv, %rdi /*ім'я файлу у ebx регістр*/
        movq $O_RDONLY, %rdx /*відкриваємо файл як read-only*/
        syscall
        movq %rax, fd

        /*get len*/
        movq $8, %rax
        movq fd, %rdi
        movq $SEEK_END, %rdx
        movq $0, %rsi
        syscall
        
        movq %rax, len

        /*mmap*/
        movq $9, %rax
        movq len, %rsi
        movq $PROT_READ, %rdx
        movq $MAP_SHARED, %r10
        movq fd, %r8
        movq $0, %r9
        syscall

        movq %rax, ptr

        /*print file*/
        movq $SYS_WRITE, %rax
        movq $STDOUT, %rdi
        movq ptr, %rsi
        movq len, %rdx
        syscall

        /*munmap*/
        movq $11, %rax
        movq len, %rsi
        syscall

        /*Close file*/
        movq $3, %rax    
        movq fd, %rdi 
        syscall         

        jmp exit
exit:
        movq $SYS_EXIT, %rax
        movq $0, %rdi
        syscall
