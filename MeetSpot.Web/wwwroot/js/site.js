document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('input[type="password"]').forEach(function(input) {
        var wrapper = document.createElement('div');
        wrapper.style.cssText = 'position:relative;';
        input.parentNode.insertBefore(wrapper, input);
        wrapper.appendChild(input);
        input.style.width = '100%';
        input.style.boxSizing = 'border-box';
        input.style.paddingRight = '42px';

        var btn = document.createElement('button');
        btn.type = 'button';
        btn.innerHTML = '<i class="fas fa-eye"></i>';
        btn.style.cssText = 'position:absolute;right:12px;top:50%;transform:translateY(-50%);background:none;border:none;cursor:pointer;color:#6b7280;font-size:15px;padding:0;line-height:1;';
        wrapper.appendChild(btn);

        btn.addEventListener('click', function() {
            if (input.type === 'password') {
                input.type = 'text';
                btn.innerHTML = '<i class="fas fa-eye-slash"></i>';
            } else {
                input.type = 'password';
                btn.innerHTML = '<i class="fas fa-eye"></i>';
            }
        });
    });
});