document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById('loginForm');

    if (loginForm) {
        loginForm.addEventListener('submit', async (event) => {
            event.preventDefault();

            const email = document.getElementById('loginEmail').value;
            const password = document.getElementById('loginPassword').value;

            try {

                const response = await fetch('/Patient/Login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({ email, password }),
                });

                if (!response.ok) {
                    throw new Error('Login failed: Invalid credentials.');
                }

                window.location.href = '/Home/Index';
            } catch (error) {
                console.error('Login failed:', error.message);

                const errorMessageElement = document.getElementById('loginErrorMessage');
                errorMessageElement.textContent = error.message;
            }
        });
    }
});
