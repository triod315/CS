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

        movq (%rsp), %rbx       /* rbx = *rsp Кількість аргументів*/
        movq %rbx, argc         /* argc = %rsp */
        movq 16(%rsp), %rcx     /* rcx = rsp + 16 */
        movq %rcx, argv         /* argv = %rcx */

        cmpq $2, argc           /*if argc !=2 goto exit*/
        jne exit

        /*open file*/
        movq $2, %rax           /*номер sys_open у eax регістр*/
        movq argv, %rdi         /*ім'я файлу у ebx регістр*/
        movq $O_RDONLY, %rdx    /*відкриваємо файл як read-only*/
        syscall
        movq %rax, fd

        /*get len*/
        movq $SYS_LSEEK, %rax   /*номер lseek*/
        movq fd, %rdi           /*fd to rdi*/
        movq $SEEK_END, %rdx    /*origin*/
        movq $0, %rsi           /*off_t offset*/
        syscall
        
        movq %rax, len          /*len=lseek(fd,0, $SYS_END)*/

        /*mmap*/
        movq $SYS_MMAP, %rax    /*номер mmap*/
        movq len, %rsi          /*length to rsi*/
        movq $PROT_READ, %rdx   /*prot_read to rdx*/
        movq $MAP_SHARED, %r10  /*map_shared to r10*/
        movq fd, %r8            /*fd ro r8*/
        movq $0, %r9            /*0 to r9*/
        syscall

        movq %rax, ptr          /*ptr = mmap(NULL, len, PROT_READ, MAP_SHARED, fd, 0);*/

        /*print file*/
        movq $SYS_WRITE, %rax
        movq $STDOUT, %rdi      /*STDOUT number to rdi*/
        movq ptr, %rsi          /*pointer to rsi*/
        movq len, %rdx          /*length to rdx*/
        syscall                 /*write(stdout, ptr, len);*/

        /*munmap*/
        movq $SYS_MUNMAP, %rax  
        movq ptr, %rdi          /*pointer to rdi*/
        movq len, %rsi          /*length to rsi*/
        syscall                 /*munmap(ptr, len);*/

        /*Close file*/
        movq $SYS_CLOSE, %rax    
        movq fd, %rdi 
        syscall         

        jmp exit
exit:
        movq $SYS_EXIT, %rax
        movq $0, %rdi
        syscall
