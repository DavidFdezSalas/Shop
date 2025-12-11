import React, { useState } from 'react';

interface RegisterForm {
    nombre: string;
    email: string;
    password: string;
}

const Register: React.FC = () => {
    const [form, setForm] = useState<RegisterForm>({ nombre: '', email: '', password: '' });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        // Aquí iría la lógica para enviar el formulario
        alert('Registro enviado');
    };

    return (
        <div>
            <h1>Registro</h1>
            <form onSubmit={handleSubmit}>
                <label>
                    Nombre:
                    <input type="text" name="nombre" value={form.nombre} onChange={handleChange} required />
                </label>
                <br />
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
                <button type="submit">Registrarse</button>
            </form>
        </div>
    );
};

export default Register;