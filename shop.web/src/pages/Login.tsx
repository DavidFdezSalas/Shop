import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

interface LoginForm {
    email: string;
    password: string;
}

const Login: React.FC = () => {
    const [form, setForm] = useState<LoginForm>({ email: '', password: '' });
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        try {
            const response = await fetch('https://localhost:7049/api/v1/Auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    email: form.email,
                    password: form.password
                }),
            });

            if (!response.ok) {
                throw new Error('Credenciales incorrectas');
            }

            const data = await response.json();
            localStorage.setItem('token', data.token);
            navigate('/home');
        } catch (err: any) {
            setError(err.message || 'Error de autenticación');
        }
    };

    return (
        <div>
            <h1>Iniciar sesión</h1>
            <form onSubmit={handleSubmit}>
                <label>
                    Email:
                    <input type="email" name="email" value={form.email} onChange={handleChange} required />
                </label>
                <br />
                <label>
                    Contraseña:
                    <input type="password" name="password" value={form.password} onChange={handleChange} required />
                </label>
                <br />
                <button type="submit">Entrar</button>
            </form>
            {error && <p style={{ color: 'red' }}>{error}</p>}
        </div>
    );
};

export default Login;